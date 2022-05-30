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

public interface ICategoryRepository : IRepositoryBase<Category>
{
    Task<Category> InsertAsync(CategoryCreationData newCategory);

    Task<IEnumerable<Category>> GetCategoriesOfRecipeByIdAsync(int recipeId);
}
