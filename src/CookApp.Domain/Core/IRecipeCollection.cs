using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;

namespace CookApp.Domain.Core;

public interface IRecipeCollection
{
    public Task<RecipeWithBriefCategories> GetRecipeWithCategoriesAsync(int recipeId);

    public Task<IEnumerable<BriefRecipeWithBriefCategories>> GetBriefRecipesWithBriefCategoriesAsync(Pagination pagination);

    public Task<IEnumerable<BriefRecipeWithBriefCategories>> GetBriefRecipesWithBriefCategoriesByCategoryIdAsync(int categoryId, Pagination pagination);
    
    public Task<Recipe> AddRecipeAsync(NewRecipe newRecipe);

    public Task<Recipe> UpdateRecipeAsync(Recipe recipe);

    public Task<Recipe> DeleteRecipeAsync(int recipeId);

    public Task<RecipeWithBriefCategories> AddRecipeToCategoryAsync(int recipeId, int categoryId);

    public Task<RecipeWithBriefCategories> RemoveRecipeFromCategoryAsync(int recipeId, int categoryId);
}
