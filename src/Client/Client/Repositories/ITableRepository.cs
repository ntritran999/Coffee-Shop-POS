using System;
using System.Collections.Generic;
using Client.Models;

namespace Client.Repositories
{
    public interface ITableRepository : IRepo<Table>
    {
        IEnumerable<Table> GetByStatus(int status);
    }
}

