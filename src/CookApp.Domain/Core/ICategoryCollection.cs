using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models.DbEntities;

namespace CookApp.Domain.Core;
public interface ICategoryCollection
{
    public Task<IEnumerable<Category>> GetAll();

    public Task<Category> GetById(int id);

    public Task<Category> Add(Category category);

    public Task<Category> Update(Category category);

    public Task<Category> Delete(int id);
}
