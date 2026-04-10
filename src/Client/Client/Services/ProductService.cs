using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client.Services
{
    public class ProductService
    {
        private readonly IProductRepository _product;

        public ProductService(IProductRepository? productRepository = null)
        {
            _product = productRepository ?? new MockProductRepoitory();
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _product.GetAll();
        }

        public async Task<Product?> GetProductByID(int id)
        {
            string idString = id.ToString();
            return await _product.GetById(idString);

        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryId)
        {
            return await _product.GetByCategory(categoryId);
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await _product.GetByName(name);
        }

        public async Task<Product> AddProduct(Product item)
        {
            return await _product.Add(item);
        }

        public async Task<bool> UpdateProduct(Product item)
        {
            return await _product.Update(item);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            return await _product.Delete(id.ToString());
        }
    }
}
