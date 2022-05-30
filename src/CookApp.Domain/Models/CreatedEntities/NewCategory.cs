﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.CreatedEntities;

public class CategoryCreationData
{
    public string Name { get; init; }

    public string Description { get; init; }

    public CategoryCreationData(string name, string description = "")
    {
        Name = name;
        Description = description;
    }
}
