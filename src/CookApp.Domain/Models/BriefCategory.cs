using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models;

public class BriefCategory
{
    public int Id { get; init; }

    public string Name { get; init; }

    public BriefCategory(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
