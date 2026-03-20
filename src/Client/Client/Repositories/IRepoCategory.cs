using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Repositories
{
    public interface IRepoCategory<TData> where TData : class 
    {
        List<TData> GetAll();
    }
}
