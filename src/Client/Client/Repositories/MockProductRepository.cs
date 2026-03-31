using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class MockProductRepoitory : IProductRepository
    {
        public Task<Product> Add(Product item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAll()
        {
            return Task.FromResult<IEnumerable<Product>>([
                new Product() { ProductID = 1, Name = "Product 1", Price = 10000, Image = "/Assets/ProductSource/1.png", CategoryID = 1 },
                new Product() { ProductID = 2, Name = "Product 2", Price = 45000, Image = "/Assets/ProductSource/2.png", CategoryID = 1 },
                new Product() { ProductID = 3, Name = "Product 3", Price = 55000, Image = "/Assets/ProductSource/3.png", CategoryID = 2 },
                new Product() { ProductID = 4, Name = "Product 4", Price = 20000, Image = "/Assets/ProductSource/4.png", CategoryID = 2 },
                new Product() { ProductID = 5, Name = "Product 5", Price = 32000, Image = "/Assets/ProductSource/5.png", CategoryID = 3 },
            ]);
        }

        public Task<IEnumerable<Product>> GetByCategory(string categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetById(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Product item)
        {
            throw new NotImplementedException();
        }
    }
}
