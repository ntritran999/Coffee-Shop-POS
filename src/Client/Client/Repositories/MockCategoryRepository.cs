using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;

namespace Client.Repositories
{
    public class MockCategoryRepository : IRepoCategory<Category>
    {
        public List<Category> GetAll()
        {
            return new List<Category>()
            {
                new Category() { ID = 1, Name = "Category 1" },
                new Category() { ID = 2, Name = "Category 2" },
                new Category() { ID = 3, Name = "Category 3" }
            };
        }
    }
}
