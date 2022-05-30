using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CookApp.Domain.DataAccessInterfaces;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookApp.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly string _connectionString;

    public RecipeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Recipe?> GetSingleOrDefaultAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QuerySingleOrDefaultAsync<Recipe>("spRecipe_GetById", new { RecipeId = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Recipe> GetSingleAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QuerySingleAsync<Recipe>("spRecipe_GetById", new { RecipeId = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Recipe>> GetPageAsync(Pagination pagination)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QueryAsync<Recipe>("spRecipe_GetPage", new { pagination.PageNumber, pagination.PageSize }, commandType: CommandType.StoredProcedure);
    }

    public Task<IEnumerable<Recipe>> GetPageByCategoryIdAsync(int categoryId, Pagination pagination)
    {
        using var db = new SqlConnection(_connectionString);
        return db.QueryAsync<Recipe>("spRecipe_GetPageByCategoryId", new { CategoryId = categoryId, pagination.PageNumber, pagination.PageSize }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Recipe> InsertAsync(RecipeCreationData newRecipe)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Name", newRecipe.Name);
        parameters.Add("Description", newRecipe.Description);
        parameters.Add("Process", newRecipe.Process);
        parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("spRecipe_Insert", parameters, commandType: CommandType.StoredProcedure);

        return await GetSingleAsync(parameters.Get<int>("Id"));
    }

    public async Task<Recipe> UpdateAsync(Recipe entity)
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("spRecipe_Update", new { RecipeId = entity.Id, entity.Name, entity.Description, entity.Process }, commandType: CommandType.StoredProcedure);
        // return updated entity or argument?
        return await GetSingleAsync(entity.Id);
    }

    public async Task<Recipe> DeleteAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        var recipe = await GetSingleAsync(id);
        await db.ExecuteAsync("spRecipe_Delete", new { RecipeId = id }, commandType: CommandType.StoredProcedure);
        return recipe;
    }
    public async Task AddRecipeToCategoryAsync(int RecipeId, int CategoryId)
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("spRecipe_AddCategory", new { RecipeId, CategoryId }, commandType: CommandType.StoredProcedure);
    }
    
    public Task RemoveRecipeFromCategoryAsync(int RecipeId, int CategoryId)
    {
        using var db = new SqlConnection(_connectionString);
        return db.ExecuteAsync("spRecipe_RemoveCategory", new { RecipeId, CategoryId }, commandType: CommandType.StoredProcedure);
    }
}
