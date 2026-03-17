using System;

namespace Client.Repositories;

public interface IBillRepository : IRepo<Bill>
{
    List<Bill> GetByDate(DateTime fromDate, DateTime toDate);
    List<Bill> GetUnpaidBills();
    Bill GetByTable(string tableId);
    void Checkout(string billId, double totalAmount, double discount);
}
