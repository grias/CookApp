using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models;
using CookApp.Domain.Models.DbEntities;

namespace CookApp.Domain.UtilityClasses;
public interface IDataMapper 
{
    public BriefRecipe ToBrief(Recipe recipe);

    public BriefCategory ToBrief(Category category);
    
    public IEnumerable<BriefRecipe> ToBrief(IEnumerable<Recipe> recipes);
    
    public IEnumerable<BriefCategory> ToBrief(IEnumerable<Category> recipes);

}
