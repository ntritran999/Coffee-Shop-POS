using System;
using System.Collections.Generic;
namespace Client.Repositories
{
    public interface ITableRepository : IRepo<Table>
    {
        IEnumerable<Table> GetByStatus(int status);
    }
}

