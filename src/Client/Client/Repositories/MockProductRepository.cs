using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Repositories
{
    public class MockProductRepoitory: IProductRepository
    {
        public void Add(Product item)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAll()
        {
            return new List<Product>() { 
                new Product() { ProductID = 1, Name = "Product 1", Price = 10000, Image = "/Assets/ProductSource/1.png", CategoryID = 1 },
                new Product() { ProductID = 2, Name = "Product 2", Price = 45000, Image = "/Assets/ProductSource/2.png", CategoryID = 1 },
                new Product() { ProductID = 3, Name = "Product 3", Price = 55000, Image = "/Assets/ProductSource/3.png", CategoryID = 2 },
                new Product() { ProductID = 4, Name = "Product 4", Price = 20000, Image = "/Assets/ProductSource/4.png", CategoryID = 2 },
                new Product() { ProductID = 5, Name = "Product 5", Price = 32000, Image = "/Assets/ProductSource/5.png", CategoryID = 3 },

            };

        }

        public IEnumerable<Product> GetByCategory(string categoryId)
        {
            throw new NotImplementedException();
        }

        public Product? GetById(string itemId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void Update(Product item)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Product> IRepo<Product>.GetAll()
        {
            return GetAll();
        }
    }
}
