using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.CreatedEntities;

public class CategoryCreationDto
{
    public string Name { get; init; }

    public string? Description { get; init; }

    public CategoryCreationDto(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public CategoryCreationDto(string name)
    {
        Name = name;
    }
}
