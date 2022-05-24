using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.DbEntities;

public class Recipe : EntityBase
{
    public string Name { get; init; }

    public string Description { get; init; }

    public string Process { get; init; }

    public DateTime CreatedDate { get; init; }

    public DateTime ModifiedDate { get; init; }

    public Recipe(int id, string name, string description, string process, DateTime createdDate, DateTime modifiedDate) : base(id)
    {
        Name = name;
        Description = description;
        Process = process;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }
}
