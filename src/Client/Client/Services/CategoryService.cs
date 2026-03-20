using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public class CategoryService
    {
        private ICategoryRepository _category = new MockCategoryRepository();

        public CategoryService() { }

        public IEnumerable<Category> GetAllCategories()
        {
            return _category.GetAll();
        }
    }
}
