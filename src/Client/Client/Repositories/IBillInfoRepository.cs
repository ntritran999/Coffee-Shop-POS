using System;

namespace Client.Repositories;

public interface IBillInfoRepository : IRepo<BillInfo>
{
    List<BillInfo> GetByBillId(string billId);
    void AddItem(string billId, string productId, int count, double price, string note);
}
