using CookApp.Domain.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models;

public class RecipeWithBriefCategories
{
    public Recipe Recipe { get; init; }

    public IEnumerable<BriefCategory> Categories { get; init; }

    public RecipeWithBriefCategories(Recipe recipe, IEnumerable<BriefCategory> categories)
    {
        Recipe = recipe;
        Categories = categories;
    }
}
