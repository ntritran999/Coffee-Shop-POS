using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Repositories
{
    public class MockProductRepoitory: IRepoProduct<Product>
    {
        public List<Product> GetAll()
        {
            return new List<Product>() { 
                new Product() { ID = 1, Name = "Product 1", Price = 10000, ImageURL = "/Assets/ProductSource/1.png", CategoryID = 1 },
                new Product() { ID = 2, Name = "Product 2", Price = 45000, ImageURL = "/Assets/ProductSource/2.png", CategoryID = 1 },
                new Product() { ID = 3, Name = "Product 3", Price = 55000, ImageURL = "/Assets/ProductSource/3.png", CategoryID = 2 },
                new Product() { ID = 4, Name = "Product 4", Price = 20000, ImageURL = "/Assets/ProductSource/4.png", CategoryID = 2 },
                new Product() { ID = 5, Name = "Product 5", Price = 32000, ImageURL = "/Assets/ProductSource/5.png", CategoryID = 3 },

            };

        }

        public List<Product> GetByCategoryID(int catID)
        {
            List<Product> products = GetAll();
            return products.FindAll(p => p.CategoryID == catID);
        }

        public List<Product> GetByID(int proID)
        {
            List<Product> products = GetAll();
            return products.FindAll(p => p.ID == proID);
        }
    }
}
