using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;

namespace CookApp.Domain.DataAccessInterfaces;

public interface IRecipeRepository : IRepositoryBase<Recipe>
{
    Task<Recipe> InsertAsync(RecipeCreationData newRecipe);

    Task<IEnumerable<Recipe>> GetPageByCategoryIdAsync(int categoryId, Pagination pagination);

    Task AddRecipeToCategoryAsync(int RecipeId, int CategoryId);

    Task RemoveRecipeFromCategoryAsync(int RecipeId, int CategoryId);
}
