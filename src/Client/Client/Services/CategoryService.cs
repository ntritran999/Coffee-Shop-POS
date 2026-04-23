using Client.Models;
using Client.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _category;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _category = categoryRepository;
        }

        public CategoryService() : this(App.Services?.GetService<ICategoryRepository>() ?? new MockCategoryRepository())
        {
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _category.GetAll();
        }

        public async Task<Category?> GetCategoryByName(string categoryName)
        {
            return await _category.GetByName(categoryName);
        }

        public async Task<Category> AddCategory(Category item)
        {
            return await _category.Add(item);
        }
    }
}
