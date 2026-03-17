using System;
using System.Collections.Generic;
namespace Client.Repositories
{
    public interface ICategoryRepository : IRepo<Category>
    {
        Category? GetByName(string categoryName);
    }
}