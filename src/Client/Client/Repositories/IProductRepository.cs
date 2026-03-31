using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface IProductRepository : IRepo<Product>
    {
        Task<IEnumerable<Product>> GetByCategory(string categoryId);
        Task<IEnumerable<Product>> GetByName(string name);
    }
}


