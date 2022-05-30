using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.CreatedEntities;

public class RecipeCreationDto
{
    public string Name { get; init; }

    public string? Description { get; init; }

    public string? Process { get; init; }

    public RecipeCreationDto(string name, string process, string description)
    {
        Name = name;
        Process = process;
        Description = description;
    }

    public RecipeCreationDto(string name)
    {
        Name = name;
    }
}
