using System;
using System.Collections.Generic;
using Client.Models;

namespace Client.Repositories
{
    public interface IBillInfoRepository : IRepo<BillInfo>
    {
        IEnumerable<BillInfo> GetByBillId(string billId);
        BillInfo? GetItem(string billId, string productId);
    }
}