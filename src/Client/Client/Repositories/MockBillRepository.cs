using Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class MockBillRepository : IBillRepository
    {
        public static List<Bill> _bills = new()
        {
            new Bill { BillID = 1, DateCheckIn = DateTime.Today.AddDays(-90), DateCheckOut = DateTime.Today.AddDays(-90).AddHours(1), TableID = 1, Status = 1, TotalAmount = 80000, Discount = 0 },
            new Bill { BillID = 2, DateCheckIn = DateTime.Today.AddDays(-85), DateCheckOut = DateTime.Today.AddDays(-85).AddHours(1), TableID = 2, Status = 1, TotalAmount = 120000, Discount = 5000 },
            new Bill { BillID = 3, DateCheckIn = DateTime.Today.AddDays(-80), TableID = 3, Status = 0, TotalAmount = 0, Discount = 0 },
            new Bill { BillID = 4, DateCheckIn = DateTime.Today.AddDays(-75), DateCheckOut = DateTime.Today.AddDays(-75).AddHours(1), TableID = 4, Status = 1, TotalAmount = 95000, Discount = 0 },

            new Bill { BillID = 5, DateCheckIn = DateTime.Today.AddDays(-70), DateCheckOut = DateTime.Today.AddDays(-70).AddHours(2), TableID = 1, Status = 1, TotalAmount = 150000, Discount = 10000 },
            new Bill { BillID = 6, DateCheckIn = DateTime.Today.AddDays(-65), TableID = 2, Status = 0, TotalAmount = 0, Discount = 0 },
            new Bill { BillID = 7, DateCheckIn = DateTime.Today.AddDays(-60), DateCheckOut = DateTime.Today.AddDays(-60).AddHours(1), TableID = 3, Status = 1, TotalAmount = 110000, Discount = 0 },
            new Bill { BillID = 8, DateCheckIn = DateTime.Today.AddDays(-55), DateCheckOut = DateTime.Today.AddDays(-55).AddHours(1), TableID = 4, Status = 1, TotalAmount = 170000, Discount = 15000 },

            new Bill { BillID = 9, DateCheckIn = DateTime.Today.AddDays(-50), TableID = 1, Status = 0, TotalAmount = 0, Discount = 0 },
            new Bill { BillID = 10, DateCheckIn = DateTime.Today.AddDays(-45), DateCheckOut = DateTime.Today.AddDays(-45).AddHours(2), TableID = 2, Status = 1, TotalAmount = 130000, Discount = 0 },
            new Bill { BillID = 11, DateCheckIn = DateTime.Today.AddDays(-40), DateCheckOut = DateTime.Today.AddDays(-40).AddHours(1), TableID = 3, Status = 1, TotalAmount = 200000, Discount = 20000 },
            new Bill { BillID = 12, DateCheckIn = DateTime.Today.AddDays(-35), TableID = 4, Status = 0, TotalAmount = 0, Discount = 0 },

            new Bill { BillID = 13, DateCheckIn = DateTime.Today.AddDays(-30), DateCheckOut = DateTime.Today.AddDays(-30).AddHours(1), TableID = 1, Status = 1, TotalAmount = 90000, Discount = 0 },
            new Bill { BillID = 14, DateCheckIn = DateTime.Today.AddDays(-25), DateCheckOut = DateTime.Today.AddDays(-25).AddHours(1), TableID = 2, Status = 1, TotalAmount = 140000, Discount = 5000 },
            new Bill { BillID = 15, DateCheckIn = DateTime.Today.AddDays(-20), TableID = 3, Status = 0, TotalAmount = 0, Discount = 0 },
            new Bill { BillID = 16, DateCheckIn = DateTime.Today.AddDays(-15), DateCheckOut = DateTime.Today.AddDays(-15).AddHours(1), TableID = 4, Status = 1, TotalAmount = 160000, Discount = 0 },

            new Bill { BillID = 17, DateCheckIn = DateTime.Today.AddDays(-10), DateCheckOut = DateTime.Today.AddDays(-10).AddHours(1), TableID = 1, Status = 1, TotalAmount = 210000, Discount = 10000 },
            new Bill { BillID = 18, DateCheckIn = DateTime.Today.AddDays(-7), TableID = 2, Status = 0, TotalAmount = 0, Discount = 0 },
            new Bill { BillID = 19, DateCheckIn = DateTime.Today.AddDays(-3), DateCheckOut = DateTime.Today.AddDays(-3).AddHours(1), TableID = 3, Status = 1, TotalAmount = 180000, Discount = 0 },
            new Bill { BillID = 20, DateCheckIn = DateTime.Today.AddDays(-1), DateCheckOut = DateTime.Today.AddDays(-1).AddHours(1), TableID = 4, Status = 1, TotalAmount = 220000, Discount = 20000 }
        };

        public Task<Bill> Add(Bill item)
        {
            item.BillID = _bills.Any() ? _bills.Max(b => b.BillID) + 1 : 1;
            item.DateCheckIn = DateTime.Now;

            _bills.Add(item);

            Debug.WriteLine("ADD BILL MOCK");
            return Task.FromResult(item);
        }

        public Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out int billId)) return Task.FromResult(false);

            var bill = _bills.FirstOrDefault(b => b.BillID == billId);
            if (bill == null) return Task.FromResult(false);

            _bills.Remove(bill);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Bill>> GetAll()
        {
            return Task.FromResult<IEnumerable<Bill>>(_bills);
        }

        public Task<Bill?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out int id)) return Task.FromResult<Bill?>(null);

            return Task.FromResult(_bills.FirstOrDefault(b => b.BillID == id));
        }

        public Task<IEnumerable<Bill>> GetByDate(DateTime fromDate, DateTime toDate)
        {
            var result = _bills.Where(b => b.DateCheckIn >= fromDate && b.DateCheckIn <= toDate);
            return Task.FromResult<IEnumerable<Bill>>(result);
        }

        public Task<IEnumerable<Bill>> GetByStatus(int status)
        {
            return Task.FromResult<IEnumerable<Bill>>(_bills.Where(b => b.Status == status));
        }

        public Task<IEnumerable<Bill?>> GetByTable(string tableId)
        {
            if (!int.TryParse(tableId, out int tableIdInt)) return Task.FromResult<IEnumerable<Bill?>>(null!);

            return Task.FromResult(
                _bills.Where(b => b.TableID == tableIdInt && b.Status == 0).Cast<Bill?>());
        }

        public Task<bool> Update(Bill item)
        {
            var bill = _bills.FirstOrDefault(b => b.BillID == item.BillID);
            if (bill == null) return Task.FromResult(false);

            bill.TableID = item.TableID;
            bill.Status = item.Status;
            bill.TotalAmount = item.TotalAmount;
            bill.Discount = item.Discount;
            bill.DateCheckOut = item.DateCheckOut;

            return Task.FromResult(true);
        }
    }
}
