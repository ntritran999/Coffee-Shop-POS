using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _category;

        public CategoryService(ICategoryRepository? categoryRepository = null)
        {
            _category = categoryRepository ?? new MockCategoryRepository();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _category.GetAll();
        }
    }
}
