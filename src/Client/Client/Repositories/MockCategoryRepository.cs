using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private static readonly List<Category> _categories =
        [
            new Category { CategoryID = 1, CategoryName = "Category 1" },
            new Category { CategoryID = 2, CategoryName = "Category 2" },
            new Category { CategoryID = 3, CategoryName = "Category 3" }
        ];

        public Task<Category> Add(Category item)
        {
            var nextId = _categories.Count == 0 ? 1 : _categories.Max(c => c.CategoryID) + 1;
            var category = new Category
            {
                CategoryID = nextId,
                CategoryName = item.CategoryName
            };

            _categories.Add(category);
            return Task.FromResult(category);
        }

        public Task<bool> Delete(string id)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryID.ToString() == id);
            if (category == null)
            {
                return Task.FromResult(false);
            }

            _categories.Remove(category);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Category>> GetAll()
        {
            return Task.FromResult<IEnumerable<Category>>(_categories.ToList());
        }

        public Task<Category?> GetById(string itemId)
        {
            return Task.FromResult<Category?>(_categories.FirstOrDefault(c => c.CategoryID.ToString() == itemId));
        }

        public Task<Category?> GetByName(string categoryName)
        {
            return Task.FromResult<Category?>(_categories.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.CategoryName)
                && c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> Update(Category item)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryID == item.CategoryID);
            if (category == null)
            {
                return Task.FromResult(false);
            }

            category.CategoryName = item.CategoryName;
            return Task.FromResult(true);
        }
    }
}
