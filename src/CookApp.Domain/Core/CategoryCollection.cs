using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookApp.Domain.Models.CreatedEntities;
using CookApp.Domain.Models.DbEntities;
using CookApp.Domain.DataAccessInterfaces;
using CookApp.Domain.UtilityClasses;

namespace CookApp.Domain.Core;
public class CategoryCollection : ICategoryCollection
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryCollection(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAll() => await _categoryRepository.GetPageAsync(Pagination.All());

    public async Task<Category> GetById(int id) => await _categoryRepository.GetByIdAsync(id);

    public Task<Category> Add(Category category) => throw new NotImplementedException();

    public async Task<Category> Update(Category category) => await _categoryRepository.UpdateAsync(category);

    public async Task<Category> Delete(int id) => await _categoryRepository.DeleteAsync(id);
}
