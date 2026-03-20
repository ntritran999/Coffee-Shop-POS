using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public class ProductService
    {
        private IRepoProduct<Product> _product = new MockProductRepoitory();

        public ProductService() { }

        public List<Product> GetAllProducts()
        {
            return _product.GetAll();
        }

        public List<Product> GetProductsByID(int proID)
        {
            return _product.GetByID(proID);
        }

        public List<Product> GetProductsByCategory(int catID)
        {
            return _product.GetByCategoryID(catID);
        }
    }
}
