using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models;

public class BriefRecipeWithBriefCategories
{
    public BriefRecipe Recipe { get; init; }

    public IEnumerable<BriefCategory> Categories { get; init; }

    public BriefRecipeWithBriefCategories(BriefRecipe recipe, IEnumerable<BriefCategory> categories)
    {
        Recipe = recipe;
        Categories = categories;
    }
}
