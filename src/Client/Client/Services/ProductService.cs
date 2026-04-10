using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

    }
}
