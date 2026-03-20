using System;
using System.Collections.Generic;

namespace Client.Repositories
{
    public interface IRepo<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(string itemId);
        void Add(T item);
        void Update(T item);
        void Delete(string id);
    }
}