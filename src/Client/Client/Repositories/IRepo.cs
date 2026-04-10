using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public interface IRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(string itemId);
        Task<T> Add(T item);
        Task<bool> Update(T item);
        Task<bool> Delete(string id);
    }
}

