using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Repositories
{
    public interface IRepoProduct<TData> where TData : class
    {
        List<TData> GetAll();

        List<TData> GetByID(int proID);

        List<TData> GetByCategoryID(int catID);
    }
}
