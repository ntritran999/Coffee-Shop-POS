using Client.Models;
using Client.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
