using System;
using System.Collections.Generic;
namespace Client.Repositories
{
    public interface IBillRepository : IRepo<Bill>
    {
        IEnumerable<Bill> GetByDate(DateTime fromDate, DateTime toDate);
        IEnumerable<Bill> GetByStatus(int status);
        Bill? GetByTable(string tableId);
    }
}