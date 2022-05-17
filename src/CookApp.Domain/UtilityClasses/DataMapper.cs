using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models;
using CookApp.Domain.Models.DbEntities;

namespace CookApp.Domain.UtilityClasses;
public class DataMapper : IDataMapper
{
    public BriefRecipe ToBrief(Recipe recipe) => new BriefRecipe(recipe.Id, recipe.Name, recipe.Description);

    public BriefCategory ToBrief(Category category) => new BriefCategory(category.Id, category.Name);

    public IEnumerable<BriefRecipe> ToBrief(IEnumerable<Recipe> recipes) => recipes.Select(ToBrief);

    public IEnumerable<BriefCategory> ToBrief(IEnumerable<Category> categories) => categories.Select(ToBrief);
}
