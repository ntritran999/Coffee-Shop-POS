using System;
using System.Collections.Generic;
namespace Client.Repositories
{
    public interface IBillInfoRepository : IRepo<BillInfo, string>
    {
        IEnumerable<BillInfo> GetByBillId(string billId);
        BillInfo? GetItem(string billId, string productId);
    }
}