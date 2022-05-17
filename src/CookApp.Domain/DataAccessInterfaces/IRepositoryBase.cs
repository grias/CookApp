using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.UtilityClasses;

namespace CookApp.Domain.DataAccessInterfaces;

public interface IRepositoryBase<T> where T : EntityBase
{
    Task<T> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetPageAsync(Pagination pagination);

    Task<T> UpdateAsync(T entity);
    
    Task<T> DeleteAsync(int id);
}
