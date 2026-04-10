using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface IBillRepository : IRepo<Bill>
    {
        Task<IEnumerable<Bill>> GetByDate(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Bill>> GetByStatus(int status);
        Task<Bill?> GetByTable(string tableId);
    }
}