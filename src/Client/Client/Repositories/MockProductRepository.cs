using Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class MockProductRepoitory : IProductRepository
    {
        static public List<Product> _products = new List<Product>() {
            new Product() { ProductID = 1, Name = "Product 1", Price = 10000, Image = "/Assets/ProductSource/1.png", CategoryID = 1 },
            new Product() { ProductID = 2, Name = "Product 2", Price = 45000, Image = "/Assets/ProductSource/2.png", CategoryID = 1 },
            new Product() { ProductID = 3, Name = "Product 3", Price = 55000, Image = "/Assets/ProductSource/3.png", CategoryID = 2 },
            new Product() { ProductID = 4, Name = "Product 4", Price = 20000, Image = "/Assets/ProductSource/4.png", CategoryID = 2 },
            new Product() { ProductID = 5, Name = "Product 5", Price = 32000, Image = "/Assets/ProductSource/5.png", CategoryID = 3 },
        };

        public MockProductRepoitory() { }
        public Task<Product> Add(Product item)
        {
            int newId = _products.Last().ProductID + 1;

            item.ProductID = newId;
            _products.Add(item);

            Debug.WriteLine("INFOR: ADD PRODUCT - MOCK");
            Debug.WriteLine(_products.Count());

            return Task.FromResult(item);
        }

        public Task<bool> Delete(string id)
        {
            var p = _products.FirstOrDefault(p => p.ProductID.ToString() == id);

            if (p == null) return Task.FromResult(false);
            
            _products.Remove(p);

            return Task.FromResult(true);
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
            //return Task.FromResult<IEnumerable<Product>>(_products);
        }

        public Task<IEnumerable<Product>> GetByCategory(string categoryId)
        {
            return Task.FromResult<IEnumerable<Product>>(_products.Where(p => p.CategoryID.ToString() == categoryId));
        }

        public Task<Product?> GetById(string itemId)
        {
            return Task.FromResult<Product?>(_products.FirstOrDefault(p => p.ProductID.ToString() == itemId));
        }

        public Task<IEnumerable<Product>> GetByName(string name)
        {
            return Task.FromResult<IEnumerable<Product>>(_products.Where(p => p.Name.ToLower().Contains(name.ToLower())));
        }

        public Task<bool> Update(Product item)
        {
            var p = _products.FirstOrDefault(p => p.ProductID == item.ProductID);
            
            if (p == null) return Task.FromResult(false);

            p.Name = item.Name;
            p.Price = item.Price;
            p.Unit = item.Unit;
            p.CategoryID = item.CategoryID;

            return Task.FromResult(true);
        }
    }
}
