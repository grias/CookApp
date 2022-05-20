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
public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Category?> GetSingleOrDefaultAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QuerySingleOrDefaultAsync<Category>("spCategory_GetById", new { CategoryId = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Category> GetSingleAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QuerySingleAsync<Category>("spCategory_GetById", new { CategoryId = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Category>> GetPageAsync(Pagination pagination)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QueryAsync<Category>("spCategory_GetPage", new { pagination.PageNumber, pagination.PageSize }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Category> InsertAsync(NewCategory newCategory)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Name", newCategory.Name);
        parameters.Add("Description", newCategory.Description);
        parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("spCategory_Insert", parameters, commandType: CommandType.StoredProcedure);

        return await GetSingleAsync(parameters.Get<int>("Id"));
    }

    public async Task<Category> UpdateAsync(Category entity)
    {
        using var db = new SqlConnection(_connectionString);
        var rowsAffected = await db.ExecuteAsync("spCategory_Update", new { CategoryId = entity.Id, entity.Name, entity.Description }, commandType: CommandType.StoredProcedure);
        // return updated entity or argument?
        return await GetSingleAsync(entity.Id);
    }
    public async Task<Category> DeleteAsync(int id)
    {
        using var db = new SqlConnection(_connectionString);
        var category = await GetSingleAsync(id);
        await db.ExecuteAsync("spCategory_Delete", new { CategoryId = id }, commandType: CommandType.StoredProcedure);
        return category;
    }
    public async Task<IEnumerable<Category>> GetCategoriesOfRecipeByIdAsync(int recipeId)
    {
        using var db = new SqlConnection(_connectionString);
        return await db.QueryAsync<Category>("spCategory_GetAllByRecipeId", new { RecipeId = recipeId }, commandType: CommandType.StoredProcedure);
    }
}
