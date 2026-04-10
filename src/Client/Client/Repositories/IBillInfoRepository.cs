using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface IBillInfoRepository : IRepo<BillInfo>
    {
        Task<IEnumerable<BillInfo>> GetByBillId(string billId);
        Task<BillInfo?> GetItem(string billId, string productId);
    }
}