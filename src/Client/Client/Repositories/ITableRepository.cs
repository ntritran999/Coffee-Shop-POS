using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface ITableRepository : IRepo<Table>
    {
        Task<IEnumerable<Table>> GetByStatus(int status);
    }
}

