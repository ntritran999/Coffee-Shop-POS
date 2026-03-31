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
        private IProductRepository _product = new MockProductRepoitory();

        public ProductService() { }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _product.GetAll();
        }

    }
}
