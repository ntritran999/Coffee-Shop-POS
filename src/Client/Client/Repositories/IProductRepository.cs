using System;

namespace Client.Repositories;

public interface IProductRepository : IRepo<Product>
{
    List<Product> GetByCategory(string categoryId);
    List<Product> SearchByName(string keyword);
}
