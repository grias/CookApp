using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Infrastructure.Repositories;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.UtilityClasses;
using CookApp.Domain.DataAccessInterfaces;
using Xunit;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Infrastructure.IntegrationTests;
internal class RecipeRepositoryTests
{
    private readonly string _connectionString;

    private readonly IRecipeRepository _recipeRepository;

    public RecipeRepositoryTests()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
        _recipeRepository = new RecipeRepository(_connectionString);
    }

    private readonly List<Recipe> _fakeRecipes = new List<Recipe>()
    {
        new Recipe(-1, "Coconut cocktail", "Tasty Coconut cocktail", "How to cook Coconut cocktail"),
        new Recipe(-1, "Apple pie", "Tasty apple pie", "How to cook apple pie"),
        new Recipe(-1, "Beef stew", "Tasty beef stew", "How to cook beef stew"),
        new Recipe(-1, "Dumplings", "Tasty dumplings", "How to cook dumplings"),
        new Recipe(-1, "Apple pie", "Tasty apple pie", "How to cook apple pie"),
    };

    public async Task ClearRecipeTable()
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("DELETE FROM Recipe");
    }

    public async Task<List<int>> PopulateRecipeTableByFakeData()
    {
        using var db = new SqlConnection(_connectionString);
        var ids = new List<int>();
        foreach (var recipe in _fakeRecipes)
        {
            var id = await db.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Recipe (Name, Description, Process) VALUES (@Name, @Description, @Process); SELECT CAST(SCOPE_IDENTITY() as int)",
                recipe);
            ids.Add(id);
        }
        return ids;
    }
}
