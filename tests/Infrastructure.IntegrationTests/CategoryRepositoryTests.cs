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
        new Category(-1, "Salads", "Tasty salads"),
        new Category(-1, "Cocktails", "Tasty cocktails"),
        new Category(-1, "Soups", "Tasty soups"),
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
    public async void GetByIdAsync_ExistingId_ReturnsCorrectCategory()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetByIdAsync(ids[0]);

        // Assert
        Assert.Equal(ids[0], category.Id);
        Assert.Equal(_fakeCategories[0].Name, category.Name);
        Assert.Equal(_fakeCategories[0].Description, category.Description);
    }

    [Fact]
    public async void GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        var ids = await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetByIdAsync(ids.Last() + 1);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var categoryRepository = new CategoryRepository(_connectionString);
        await ClearCategoryTable();
        await PopulateCategoryTableByFakeData();

        // Act
        var category = await categoryRepository.GetByIdAsync(-1);

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
}
