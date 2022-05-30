using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models;
using CookApp.Domain.DataAccessInterfaces;
using CookApp.Domain.UtilityClasses;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;

namespace CookApp.Domain.Core;

public class RecipeCollection : IRecipeCollection
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDataMapper _dataMapper;

    public RecipeCollection(IRecipeRepository recipeRepository, ICategoryRepository categoryRepository, IDataMapper dataMapper)
    {
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _dataMapper = dataMapper;
    }

    public async Task<RecipeWithBriefCategories> GetRecipeWithCategoriesAsync(int recipeId)
    {
        var recipe = await _recipeRepository.GetSingleOrDefaultAsync(recipeId);
        var categories = await _categoryRepository.GetCategoriesOfRecipeByIdAsync(recipeId);

        return new RecipeWithBriefCategories(recipe, _dataMapper.ToBrief(categories));
    }

    public async Task<IEnumerable<BriefRecipeWithBriefCategories>> GetBriefRecipesWithBriefCategoriesAsync(Pagination pagination)
    {

        var recipes = await _recipeRepository.GetPageAsync(pagination);
        return await AttachBriefCategoriesToBriefRecipes(_dataMapper.ToBrief(recipes));
    }

    public async Task<IEnumerable<BriefRecipeWithBriefCategories>> GetBriefRecipesWithBriefCategoriesByCategoryIdAsync(int categoryId, Pagination pagination)
    {
        var recipes = await _recipeRepository.GetPageByCategoryIdAsync(categoryId, pagination);
        return await AttachBriefCategoriesToBriefRecipes(_dataMapper.ToBrief(recipes));
    }

    private async Task<IEnumerable<BriefRecipeWithBriefCategories>> AttachBriefCategoriesToBriefRecipes(IEnumerable<BriefRecipe> briefRecepies)
    {
        async Task<BriefRecipeWithBriefCategories> CreateBriefRecipeWithCategoriesAsync(BriefRecipe recipe)
        {
            var categories = await _categoryRepository.GetCategoriesOfRecipeByIdAsync(recipe.Id);

            return new BriefRecipeWithBriefCategories(recipe, _dataMapper.ToBrief(categories));
        }

        /**
         * TODO: Optimize calls to database
         */

        return await Task.WhenAll(briefRecepies.Select(recipe => CreateBriefRecipeWithCategoriesAsync(recipe)));
    }

    public async Task<Recipe> AddRecipeAsync(RecipeCreationData newRecipe) => await _recipeRepository.InsertAsync(newRecipe);

    public async Task<Recipe> UpdateRecipeAsync(Recipe recipe) => await _recipeRepository.UpdateAsync(recipe);

    public async Task<Recipe> DeleteRecipeAsync(int recipeId) => await _recipeRepository.DeleteAsync(recipeId);

    public async Task<RecipeWithBriefCategories> AddRecipeToCategoryAsync(int recipeId, int categoryId)
    {
        await _recipeRepository.AddRecipeToCategoryAsync(recipeId, categoryId);
        return await GetRecipeWithCategoriesAsync(recipeId);
    }

    public async Task<RecipeWithBriefCategories> RemoveRecipeFromCategoryAsync(int recipeId, int categoryId)
    {
        await _recipeRepository.RemoveRecipeFromCategoryAsync(recipeId, categoryId);
        return await GetRecipeWithCategoriesAsync(recipeId);
    }
}
