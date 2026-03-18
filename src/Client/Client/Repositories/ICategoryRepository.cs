using System;
using System.Collections.Generic;
using Client.Models;

namespace Client.Repositories
{
    public interface ICategoryRepository : IRepo<Category>
    {
        Category? GetByName(string categoryName);
    }
}