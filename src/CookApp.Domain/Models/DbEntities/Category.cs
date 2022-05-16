using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.DbEntities;

public class Category : EntityBase
{
    public string Name { get; init; }
    public string Description { get; init; }
    public Category(int id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }
}
