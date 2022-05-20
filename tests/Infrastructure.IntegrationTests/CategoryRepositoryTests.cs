using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Infrastructure.Repositories;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.UtilityClasses;
using Xunit;
using Dapper;
using Microsoft.Data.SqlClient;


namespace Infrastructure.IntegrationTests;

public class CategoryRepositoryTests
{

    private readonly string _connectionString;

    public CategoryRepositoryTests()
    {
        _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
    }

    private readonly List<Category> _fakeCategories = new List<Category>()
    {
        new Category(-1, "Cakes", "Tasty cakes"),
        new Category(-1, "Eggs", "Tasty eggs"),
        new Category(-1, "Ales", "Tasty ales"),
        new Category(-1, "Ducks", "Tasty ducks"),
        new Category(-1, "Burgers", "Tasty burgers")
    };

    public async Task ClearCategoryTable()
    {
        using var db = new SqlConnection(_connectionString);
        await db.ExecuteAsync("DELETE FROM Category");
    }

    public async Task<List<int>> PopulateCategoryTableByFakeData()
    {
        using var db = new SqlConnection(_connectionString);
        var ids = new List<int>();
        foreach (var category in _fakeCategories)
        {
            var id = await db.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Category (Name, Description) VALUES (@Name, @Description); SELECT CAST(SCOPE_IDENTITY() as int)",
                category);
            ids.Add(id);
        }
        return ids;
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
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetSingleOrDefaultAsync(ids[0]);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(ids[0], category!.Id);
        Assert.Equal(_fakeCategories[0].Name, category.Name);
        Assert.Equal(_fakeCategories[0].Description, category.Description);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetSingleOrDefaultAsync(ids.Last() + 1);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void GetSingleOrDefaultAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetSingleOrDefaultAsync(-1);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void GetPageAsync_All_ReturnsAllCategories()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();

        // Act
        var categories = (await categoryRepository.GetPageAsync(Pagination.All)).ToList();

        // Assert
        Assert.Equal(_fakeCategories.Count, categories.Count);
    }

    [Fact]
    public async void GetPageAsync_SecondPage_ReturnsCorrectCategories()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();
        var sortedFakeCategories = _fakeCategories.OrderBy(c => c.Name).ToList();
        int pageNumber = 2;
        int pageSize = 2;
        var secondPage = new Pagination(pageNumber, pageSize);

        // Act
        var categories = (await categoryRepository.GetPageAsync(secondPage)).ToList();

        // Assert
        Assert.Equal(2, categories.Count);
        Assert.Equal(sortedFakeCategories[2].Name, categories[0].Name);
        Assert.Equal(sortedFakeCategories[3].Name, categories[1].Name);
    }

    [Fact]
    public async void GetPageAsync_NonexistentPage_ReturnsEmptyCollection()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();

        // Act
        var categories = (await categoryRepository.GetPageAsync(new Pagination(int.MaxValue, 1))).ToList();

        // Assert
        Assert.Empty(categories);
    }

    [Fact]
    public async void InsertAsync_newCategory_ReturnsInsertedCategory()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();

        // Act
        var category = new NewCategory("CorrectName", "CorrectDescription");
        var insertedCategory = await categoryRepository.InsertAsync(category);

        // Assert
        Assert.Equal(category.Name, insertedCategory.Name);
        Assert.Equal(category.Description, insertedCategory.Description);
    }

    [Fact]
    public async void InsertAsync_newCategory_InsertsCorrectCategory()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();

        // Act
        var category = new NewCategory("CorrectName", "CorrectDescription");
        await categoryRepository.InsertAsync(category);

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
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();

        // Act
        var category = new NewCategory(_fakeCategories[0].Name, "CorrectDescription");
        var action = async () => await categoryRepository.InsertAsync(category);

        // Assert
        await Assert.ThrowsAsync<SqlException>(action);
    }

    [Fact]
    public async void UpdateAsync_ExistingCategory_UpdatesCategory()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var categoryUpdate = new Category(ids[0], "CorrectName", "CorrectDescription");
        await categoryRepository.UpdateAsync(categoryUpdate);

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
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var categoryUpdate = new Category(ids[0], "CorrectName", "CorrectDescription");
        var updatedCategory = await categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        Assert.Equal(categoryUpdate.Id, updatedCategory.Id);
        Assert.Equal(categoryUpdate.Name, updatedCategory.Name);
        Assert.Equal(categoryUpdate.Description, updatedCategory.Description);
    }

    [Fact]
    public async void UpdateAsync_NonExistingCategory_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var categoryUpdate = new Category(ids.Last() + 1, "CorrectName", "CorrectDescription");
        var action = async () => await categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async void UpdateAsync_ExistingName_ThrowsSqlException()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var categoryUpdate = new Category(ids[0], _fakeCategories[1].Name, "CorrectDescription");
        var action = async () => await categoryRepository.UpdateAsync(categoryUpdate);

        // Assert
        await Assert.ThrowsAsync<SqlException>(action);
    }
}
