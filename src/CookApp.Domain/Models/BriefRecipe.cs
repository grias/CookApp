using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models;

public class BriefRecipe
{
    public int Id { get; init; }

    public string Name { get; init; }

    public string Description { get; init; }

    public BriefRecipe(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
