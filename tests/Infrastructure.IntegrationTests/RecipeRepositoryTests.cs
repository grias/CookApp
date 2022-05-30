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
public class RecipeRepositoryTests
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
        new Recipe(-1, "Coconut cocktail", "Tasty Coconut cocktail", "How to cook Coconut cocktail", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Ecler", "Tasty ecler", "How to cook ecler", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Beef stew", "Tasty beef stew", "How to cook beef stew", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Dumplings", "Tasty dumplings", "How to cook dumplings", DateTime.Now, DateTime.Now),
        new Recipe(-1, "Apple pie", "Tasty apple pie", "How to cook apple pie", DateTime.Now, DateTime.Now),
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

    [Fact]
    public async void GetSingleOrDefaultAsync_ValidId_ReturnsRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];

        // Act
        var recipe = await _recipeRepository.GetSingleOrDefaultAsync(id);

        // Assert
        Assert.NotNull(recipe);
        Assert.Equal(id, recipe!.Id);
        Assert.Equal(_fakeRecipes[0].Name, recipe.Name);
        Assert.Equal(_fakeRecipes[0].Description, recipe.Description);
        Assert.Equal(_fakeRecipes[0].Process, recipe.Process);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_NonexistentId_ReturnsNull()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var nonexistentId = ids.Last() + 1;

        // Act
        var recipe = await _recipeRepository.GetSingleOrDefaultAsync(nonexistentId);

        // Assert
        Assert.Null(recipe);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_IncorrectId_ReturnsNull()
    {
        // Arrange
        await ClearRecipeTable();
        await PopulateRecipeTableByFakeData();
        var incorrectId = -1;

        // Act
        var recipe = await _recipeRepository.GetSingleOrDefaultAsync(incorrectId);

        // Assert
        Assert.Null(recipe);
    }

    [Fact]
    public async void GetSingleAsync_ValidId_ReturnsRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];

        // Act
        var recipe = await _recipeRepository.GetSingleAsync(id);

        // Assert
        Assert.NotNull(recipe);
        Assert.Equal(id, recipe!.Id);
        Assert.Equal(_fakeRecipes[0].Name, recipe.Name);
        Assert.Equal(_fakeRecipes[0].Description, recipe.Description);
        Assert.Equal(_fakeRecipes[0].Process, recipe.Process);
    }

    [Fact]
    public async void GetSingleAsync_NonexistentId_ThrowsException()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var nonexistentId = ids.Last() + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _recipeRepository.GetSingleAsync(nonexistentId));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void GetSingleAsync_IncorrectId_ThrowsException()
    {
        // Arrange
        await ClearRecipeTable();
        await PopulateRecipeTableByFakeData();
        var incorrectId = -1;

        // Act
        var exception = await Record.ExceptionAsync(() => _recipeRepository.GetSingleAsync(incorrectId));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void GetPageAsync_All_ReturnsAllRecipes()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var all = Pagination.All;

        // Act
        var recipes = (await _recipeRepository.GetPageAsync(all)).ToList();

        // Assert
        Assert.NotNull(recipes);
        Assert.Equal(_fakeRecipes.Count, recipes.Count);
    }

    [Fact]
    public async void GetPageAsync_SecondPage_ReturnsCorrectRecipes()
    {
        // Arrange
        await ClearRecipeTable();
        await PopulateRecipeTableByFakeData();
        var secondPage = new Pagination(2, 2);
        var correctRecipes = (await _recipeRepository.GetPageAsync(Pagination.All))
            .OrderByDescending(r => r.ModifiedDate).Skip(2).Take(2).ToList();

        // Act
        var recipes = (await _recipeRepository.GetPageAsync(secondPage)).ToList();

        // Assert
        Assert.Equal(2, recipes.Count);
        Assert.Equal(correctRecipes[0].Name, recipes[0].Name);
        Assert.Equal(correctRecipes[1].Name, recipes[1].Name);
    }

    [Fact]
    public async void GetPageAsync_NonexistentPage_ReturnsEmptyCollection()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var nonexistentPage = new Pagination(ids.Count, 2);

        // Act
        var recipes = (await _recipeRepository.GetPageAsync(nonexistentPage)).ToList();

        // Assert
        Assert.Empty(recipes);
    }

    [Fact]
    public async void InsertAsync_ValidData_InsertsRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var newRecipe = new RecipeCreationData("Valid name", "Valid cooking process");

        // Act
        await _recipeRepository.InsertAsync(newRecipe);

        // Assert
        using var db = new SqlConnection(_connectionString);
        var insertedRecipe = await db.QuerySingleOrDefaultAsync<Recipe>(
            "SELECT * FROM Recipe WHERE Name = @Name AND Process = @Process",
            new { newRecipe.Name, newRecipe.Process });
        Assert.NotNull(insertedRecipe);
    }

    [Fact]
    public async void InsertAsync_ValidData_ReturnsInsertedRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var newRecipe = new RecipeCreationData("Valid name", "Valid cooking process");


        // Act
        var insertedRecipe = await _recipeRepository.InsertAsync(newRecipe);

        // Assert
        Assert.NotNull(insertedRecipe);
        Assert.Equal(newRecipe.Name, insertedRecipe!.Name);
        Assert.Equal(newRecipe.Process, insertedRecipe!.Process);
    }

    [Fact]
    public async void UpdateAsync_ExistingRecipe_UpdatesRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];
        var arbitraryDateTime = DateTime.Now;
        var updatedRecipe = new Recipe(id, "Updated name", "Updated cooking process", "Updated process", arbitraryDateTime, arbitraryDateTime);

        // Act
        await _recipeRepository.UpdateAsync(updatedRecipe);

        // Assert
        using var db = new SqlConnection(_connectionString);
        var updatedRecipeFromDb = await db.QuerySingleOrDefaultAsync<Recipe>(
            "SELECT * FROM Recipe WHERE Id = @Id",
            new { id });
        Assert.NotNull(updatedRecipeFromDb);
        Assert.Equal(updatedRecipe.Name, updatedRecipeFromDb.Name);
        Assert.Equal(updatedRecipe.Process, updatedRecipeFromDb.Process);
        Assert.Equal(updatedRecipe.Description, updatedRecipeFromDb.Description);
    }

    [Fact]
    public async void UpdateAsync_ExistingRecipe_ReturnsUpdatedRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];
        var arbitraryDateTime = DateTime.Now;
        var updatedRecipe = new Recipe(id, "Updated name", "Updated cooking process", "Updated process", arbitraryDateTime, arbitraryDateTime);

        // Act
        var updatedRecipeFromDb = await _recipeRepository.UpdateAsync(updatedRecipe);

        // Assert
        Assert.NotNull(updatedRecipeFromDb);
        Assert.Equal(updatedRecipe.Name, updatedRecipeFromDb!.Name);
        Assert.Equal(updatedRecipe.Process, updatedRecipeFromDb!.Process);
        Assert.Equal(updatedRecipe.Description, updatedRecipeFromDb!.Description);
    }

    [Fact]
    public async void UpdateAsync_NonexistingRecipe_ThrowsInvalidOperationException()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids.Last() + 1;
        var arbitraryDateTime = DateTime.Now;
        var updatedRecipe = new Recipe(id, "Updated name", "Updated cooking process", "Updated process", arbitraryDateTime, arbitraryDateTime);

        // Act
        var exception = await Record.ExceptionAsync(() => _recipeRepository.UpdateAsync(updatedRecipe));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public async void DeleteAsync_ExistingRecipe_DeletesRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];

        // Act
        await _recipeRepository.DeleteAsync(id);

        // Assert
        using var db = new SqlConnection(_connectionString);
        var deletedRecipe = await db.QuerySingleOrDefaultAsync<Recipe>(
            "SELECT * FROM Recipe WHERE Id = @Id",
            new { id });
        Assert.Null(deletedRecipe);
    }

    [Fact]
    public async void DeleteAsync_ExistingRecipe_ReturnsDeletedRecipe()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids[0];

        // Act
        var deletedRecipe = await _recipeRepository.DeleteAsync(id);

        // Assert
        Assert.NotNull(deletedRecipe);
        Assert.Equal(id, deletedRecipe!.Id);
    }

    [Fact]
    public async void DeleteAsync_NonexistingRecipe_ThrowsInvalidOperationException()
    {
        // Arrange
        await ClearRecipeTable();
        var ids = await PopulateRecipeTableByFakeData();
        var id = ids.Last() + 1;

        // Act
        var exception = await Record.ExceptionAsync(() => _recipeRepository.DeleteAsync(id));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }
}
