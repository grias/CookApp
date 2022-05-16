using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.Models.DbEntities;

public abstract class EntityBase
{
    public int Id { get; init; }

    public EntityBase(int id)
    {
        Id = id;
    }
}
