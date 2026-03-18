using System;
using System.Collections.Generic;
using Client.Models;

namespace Client.Repositories
{
    public interface IProductRepository : IRepo<Product>
    {
        IEnumerable<Product> GetByCategory(string categoryId);
        IEnumerable<Product> GetByName(string name);
    }
}


