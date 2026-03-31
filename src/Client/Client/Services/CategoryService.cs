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
        private ICategoryRepository _category = new MockCategoryRepository();

        public CategoryService() { }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _category.GetAll();
        }
    }
}
