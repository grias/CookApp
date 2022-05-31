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

public class CategoryRepositoryTests
{

    private readonly string _connectionString;

    private readonly FakeData _fd;

    private readonly ICategoryRepository _categoryRepository;

    public CategoryRepositoryTests()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
        _categoryRepository = new CategoryRepository(_connectionString);
        _fd = new FakeData(_connectionString);
    }

    [Fact]
    public async void ConnectionEstablished()
    {
        // Arrange
        using var db = new SqlConnection(_connectionString);

        // Act
        await db.OpenAsync();

        // Assert
        Assert.True(db.State == System.Data.ConnectionState.Open);

        
        await db.CloseAsync();
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_ExistingId_ReturnsCorrectCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();

        // Act
        var category = await _categoryRepository.GetSingleOrDefaultAsync(ids[0]);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(ids[0], category!.Id);
        Assert.Equal(_fd.FakeCategories[0].Name, category.Name);
        Assert.Equal(_fd.FakeCategories[0].Description, category.Description);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();

        // Act
        var category = await _categoryRepository.GetSingleOrDefaultAsync(ids.Last() + 1);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        await _fd.ClearAllTables();
        await _fd.PopulateCategoryTableByFakeData();

        // Act
        var category = await _categoryRepository.GetSingleOrDefaultAsync(-1);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void GetPageAsync_All_ReturnsAllCategories()
    {
        // Arrange
        await _fd.ClearAllTables();
        await _fd.PopulateCategoryTableByFakeData();

        // Act
        var categories = (await _categoryRepository.GetPageAsync(Pagination.All)).ToList();

        // Assert
        Assert.Equal(_fd.FakeCategories.Count, categories.Count);
    }

    [Fact]
    public async void GetPageAsync_SecondPage_ReturnsCorrectCategories()
    {
        // Arrange
        await _fd.ClearAllTables();
        await _fd.PopulateCategoryTableByFakeData();
        var sortedFakeCategories = _fd.FakeCategories.OrderBy(c => c.Name).ToList();
        int pageNumber = 2;
        int pageSize = 2;
        var secondPage = new Pagination(pageNumber, pageSize);

        // Act
        var categories = (await _categoryRepository.GetPageAsync(secondPage)).ToList();

        // Assert
        Assert.Equal(2, categories.Count);
        Assert.Equal(sortedFakeCategories[2].Name, categories[0].Name);
        Assert.Equal(sortedFakeCategories[3].Name, categories[1].Name);
    }

    [Fact]
    public async void GetPageAsync_NonexistentPage_ReturnsEmptyCollection()
    {
        // Arrange
        await _fd.ClearAllTables();
        await _fd.PopulateCategoryTableByFakeData();

        // Act
        var categories = (await _categoryRepository.GetPageAsync(new Pagination(int.MaxValue, 1))).ToList();

        // Assert
        Assert.Empty(categories);
    }

    [Fact]
    public async void InsertAsync_newCategory_ReturnsInsertedCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var category = new CategoryCreationDto("CorrectName", "CorrectDescription");

        // Act
        var insertedCategory = await _categoryRepository.InsertAsync(category);

        // Assert
        Assert.Equal(category.Name, insertedCategory.Name);
        Assert.Equal(category.Description, insertedCategory.Description);
    }

    [Fact]
    public async void InsertAsync_newCategory_InsertsCorrectCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var category = new CategoryCreationDto("CorrectName", "CorrectDescription");


        // Act
        await _categoryRepository.InsertAsync(category);

        // Assert
        
        using var db = new SqlConnection(_connectionString);
        var insertedCategory = await db.QuerySingleOrDefaultAsync<Category>(
            "SELECT * FROM Category WHERE Name = @Name AND Description = @Description",
            new { category.Name, category.Description });
        Assert.NotNull(insertedCategory);
    }

    [Fact]
    public async void InsertAsync_ExistingName_ThrowsSqlException()
    {
        // Arrange
        await _fd.ClearAllTables();
        await _fd.PopulateCategoryTableByFakeData();
        var category = new CategoryCreationDto(_fd.FakeCategories[0].Name, "CorrectDescription");

        // Act
        var action = async () => await _categoryRepository.InsertAsync(category);

        // Assert
        await Assert.ThrowsAsync<SqlException>(action);
    }

    [Fact]
    public async void UpdateAsync_ExistingCategory_UpdatesCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();
        var categoryUpdate = new Category(ids[0], "CorrectName", "CorrectDescription");

        // Act
        await _categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        using var db = new SqlConnection(_connectionString);
        var updatedCategory = await db.QuerySingleOrDefaultAsync<Category>(
            "SELECT * FROM Category WHERE Id = @Id", new { Id = ids[0] });

        Assert.Equal(categoryUpdate.Id, updatedCategory.Id);
        Assert.Equal(categoryUpdate.Name, updatedCategory.Name);
        Assert.Equal(categoryUpdate.Description, updatedCategory.Description);
    }

    [Fact]
    public async void UpdateAsync_ExistingCategory_ReturnsUpdatedCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();
        var categoryUpdate = new Category(ids[0], "CorrectName", "CorrectDescription");

        // Act
        var updatedCategory = await _categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        Assert.Equal(categoryUpdate.Id, updatedCategory.Id);
        Assert.Equal(categoryUpdate.Name, updatedCategory.Name);
        Assert.Equal(categoryUpdate.Description, updatedCategory.Description);
    }

    [Fact]
    public async void UpdateAsync_NonExistingCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();
        var categoryUpdate = new Category(ids.Last() + 1, "CorrectName", "CorrectDescription");

        // Act
        var action = async () => await _categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async void UpdateAsync_ExistingName_ThrowsSqlException()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();
        var categoryUpdate = new Category(ids[0], _fd.FakeCategories[1].Name, "CorrectDescription");

        // Act
        var action = async () => await _categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        await Assert.ThrowsAsync<SqlException>(action);
    }

    [Fact]
    public async void DeleteAsync_ExistingCategory_DeletesCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();

        // Act
        await _categoryRepository.DeleteAsync(ids[0]);

        // Assert
        using var db = new SqlConnection(_connectionString);
        var deletedCategory = await db.QuerySingleOrDefaultAsync<Category>(
            "SELECT * FROM Category WHERE Id = @Id", new { Id = ids[0] });
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async void DeleteAsync_ExistingCategory_ReturnsDeletedCategory()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();

        // Act
        var deletedCategory = await _categoryRepository.DeleteAsync(ids[0]);

        // Assert
        Assert.Equal(ids[0], deletedCategory.Id);
        Assert.Equal(_fd.FakeCategories[0].Name, deletedCategory.Name);
        Assert.Equal(_fd.FakeCategories[0].Description, deletedCategory.Description);
    }

    [Fact]
    public async void DeleteAsync_NonExistingCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fd.ClearAllTables();
        var ids = await _fd.PopulateCategoryTableByFakeData();

        // Act
        var action = async () => await _categoryRepository.DeleteAsync(ids.Last() + 1);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async void GetCategoriesOfRecipeByIdAsync_ValidId_ReturnsCategories()
    {
        // Arrange
        await _fd.ClearAllTables();
        var (recipeId, categoryIds) = await _fd.CreateRecipeCategoryConnections();

        // Act
        var categories = (await _categoryRepository.GetCategoriesOfRecipeByIdAsync(recipeId)).ToList();

        // Assert
        Assert.Equal(2, categories.Count());
        Assert.Equal(categoryIds[0], categories[0].Id);
        Assert.Equal(categoryIds[1], categories[1].Id);
    }
}
