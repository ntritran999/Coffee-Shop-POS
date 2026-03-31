using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface ICategoryRepository : IRepo<Category>
    {
        Task<Category?> GetByName(string categoryName);
    }
}