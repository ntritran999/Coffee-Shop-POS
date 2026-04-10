using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;

namespace Client.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
       
        public IEnumerable<Category> GetAll()
        {
            return new List<Category>()
            {
                new Category() { CategoryID = 1, CategoryName = "Category 1" },
                new Category() { CategoryID = 2, CategoryName = "Category 2" },
                new Category() { CategoryID = 3, CategoryName = "Category 3" }
            };
        }
        public Category GetByName(string name)
        {
            throw new NotImplementedException();
        }
        public Category GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Add(Category item)
        {
            throw new NotImplementedException();
        }

        public void Update(Category item)
        {
            throw new NotImplementedException();
        }
        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
