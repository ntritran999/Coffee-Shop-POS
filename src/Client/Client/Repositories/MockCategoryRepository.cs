using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        public Task<Category> Add(Category item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetAll()
        {
            return Task.FromResult<IEnumerable<Category>>(
            [
                new Category() { CategoryID = 1, CategoryName = "Category 1" },
                new Category() { CategoryID = 2, CategoryName = "Category 2" },
                new Category() { CategoryID = 3, CategoryName = "Category 3" }
            ]);
        }

        public Task<Category?> GetById(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<Category?> GetByName(string categoryName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Category item)
        {
            throw new NotImplementedException();
        }
    }
}
