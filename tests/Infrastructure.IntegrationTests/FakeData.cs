using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using CookApp.Domain.Models.DbEntities;


namespace Infrastructure.IntegrationTests;
internal class FakeData
{
    private readonly string _connectionString;

    public List<Recipe> FakeRecipes { get; init; } = new List<Recipe>()
    {
        new Recipe(-1, "Coconut cocktail", "Tasty Coconut cocktail", "How to cook Coconut cocktail", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Ecler", "Tasty ecler", "How to cook ecler", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Beef stew", "Tasty beef stew", "How to cook beef stew", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Dumplings", "Tasty dumplings", "How to cook dumplings", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Apple pie", "Tasty apple pie", "How to cook apple pie", DateTime.Now, DateTime.Now),
    };

    public List<Category> FakeCategories { get; init; } = new List<Category>()
    {
        new Category(-1, "Cakes", "Tasty cakes"),
        new Category(-1, "Eggs", "Tasty eggs"),
        new Category(-1, "Ales", "Tasty ales"),
        new Category(-1, "Ducks", "Tasty ducks"),
        new Category(-1, "Burgers", "Tasty burgers")
    };

    public FakeData(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ClearAllTables()
    {
        await ClearRecipeTable();
        await ClearCategoryTable();
        await ClearRecipeCategoryTable();
    }

    public async Task<List<int>> PopulateRecipeTableByFakeData()
    {
        using var db = new SqlConnection(_connectionString);
        var ids = new List<int>();
        foreach (var recipe in FakeRecipes)
        {
            var id = await db.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Recipe (Name, Description, Process) VALUES (@Name, @Description, @Process); SELECT CAST(SCOPE_IDENTITY() as int)",
                recipe);
            ids.Add(id);
        }
        return ids;
    }

    public async Task<List<int>> PopulateCategoryTableByFakeData()
    {
        using var db = new SqlConnection(_connectionString);
        var ids = new List<int>();
        foreach (var category in FakeCategories)
        {
            var id = await db.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Category (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int)",
                category);
            ids.Add(id);
        }
        return ids;
    }

    public async Task<(int RecipeId, List<int> CategoryIds)> CreateRecipeCategoryConnections()
    {
        var recipeIds = await PopulateRecipeTableByFakeData();
        var categoryIds = await PopulateCategoryTableByFakeData();

        using var db = new SqlConnection(_connectionString);

        await db.ExecuteAsync(
                "INSERT INTO RecipeCategory (RecipeId, CategoryId) VALUES (@RecipeId, @CategoryId)",
                new { RecipeId = recipeIds[0], CategoryId = categoryIds[0] });

        await db.ExecuteAsync(
                "INSERT INTO RecipeCategory (RecipeId, CategoryId) VALUES (@RecipeId, @CategoryId)",
                new { RecipeId = recipeIds[0], CategoryId = categoryIds[1] });

        return (recipeIds[0], new List<int> { categoryIds[0], categoryIds[1] });
    }

    private async Task ClearRecipeTable()
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("DELETE FROM Recipe");
    }

    private async Task ClearCategoryTable()
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("DELETE FROM Category");
    }

    private async Task ClearRecipeCategoryTable()
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("DELETE FROM RecipeCategory");
    }    

}
