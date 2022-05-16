using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.CreatedEntities;

public class NewRecipe
{
    public string Name { get; init; }

    public string Description { get; init; }

    public string Process { get; init; }

    public NewRecipe(string name, string process, string description = "")
    {
        Name = name;
        Process = process;
        Description = description;
    }
}
