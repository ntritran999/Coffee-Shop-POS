using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public class ProductService
    {
        private IProductRepository _product = new MockProductRepoitory();

        public ProductService() { }

        public IEnumerable<Product> GetAllProducts()
        {
            return _product.GetAll();
        }

    }
}
