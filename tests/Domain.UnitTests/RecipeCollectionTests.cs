using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CookApp.Domain.Core;
using CookApp.Domain.Models;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.DataAccessInterfaces;
using CookApp.Domain.UtilityClasses;


namespace Domain.UnitTests;

public class RecipeCollectionTests
{
    [Fact]
    public async void GetRecipeWithCategoriesAsync_ValidId_ReturnsValidRecipeWithBriefCategories()
    {
        // Arrange
        var validRecipe = new Recipe(1, "Test Recipe", "Test Description", "Test Process", DateTime.Now, DateTime.Now);
        var validCategories = new List<Category> {
            new Category(1, "Test Category 1", "Test Description"),
            new Category(2, "Test Category 2", "Test Description")
        };
        var validBriefCategories = new List<BriefCategory> {
            new BriefCategory(1, "Test Category 1"),
            new BriefCategory(2, "Test Category 2")
        };

        var recipeRepoMock = new Mock<IRecipeRepository>();
        recipeRepoMock.Setup(x => x.GetSingleOrDefaultAsync(It.IsAny<int>()))
            .ReturnsAsync(validRecipe);
        var categoryRepoMock = new Mock<ICategoryRepository>();
        categoryRepoMock.Setup(x => x.GetCategoriesOfRecipeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(validCategories);
        var dataMapperMock = new Mock<IDataMapper>();
        dataMapperMock.Setup(x => x.ToBrief(It.IsAny<IEnumerable<Category>>())).Returns(validBriefCategories);
        var recipeCollection = new RecipeCollection(recipeRepoMock.Object, categoryRepoMock.Object, dataMapperMock.Object);

        // Act
        var result = await recipeCollection.GetRecipeWithCategoriesAsync(1);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async void GetRecipeWithCategoriesAsync_NonexistentId_ReturnsNull()
    {
        // Arrange
        Recipe? nullRecipe = null;
        var validCategories = new List<Category> {
            new Category(1, "Test Category 1", "Test Description"),
            new Category(2, "Test Category 2", "Test Description")
        };
        var validBriefCategories = new List<BriefCategory> {
            new BriefCategory(1, "Test Category 1"),
            new BriefCategory(2, "Test Category 2")
        };

        var recipeRepoMock = new Mock<IRecipeRepository>();
        recipeRepoMock.Setup(x => x.GetSingleOrDefaultAsync(It.IsAny<int>()))
            .ReturnsAsync(nullRecipe);
        var categoryRepoMock = new Mock<ICategoryRepository>();
        categoryRepoMock.Setup(x => x.GetCategoriesOfRecipeByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(validCategories);
        var dataMapperMock = new Mock<IDataMapper>();
        dataMapperMock.Setup(x => x.ToBrief(It.IsAny<IEnumerable<Category>>())).Returns(validBriefCategories);
        var recipeCollection = new RecipeCollection(recipeRepoMock.Object, categoryRepoMock.Object, dataMapperMock.Object);

        // Act
        var result = await recipeCollection.GetRecipeWithCategoriesAsync(1);

        // Assert
        Assert.Null(result);
    }
}
