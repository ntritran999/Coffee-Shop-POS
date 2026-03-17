using System;

namespace Client.Repositories;

public interface ITableRepository : IRepo<Table>
{
    List<Table> GetByStatus(int status);
}
