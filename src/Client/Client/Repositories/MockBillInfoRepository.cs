using Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class MockBillInfoRepository : IBillInfoRepository
    {
        public static List<BillInfo> _items = new()
        {
            new BillInfo { BillInfoID = 1, BillID = 1, ProductID = 1, Count = 2, Price = 10000 },
            new BillInfo { BillInfoID = 2, BillID = 1, ProductID = 2, Count = 1, Price = 45000 },
            new BillInfo { BillInfoID = 3, BillID = 2, ProductID = 3, Count = 2, Price = 10000 },
            new BillInfo { BillInfoID = 4, BillID = 2, ProductID = 4, Count = 1, Price = 45000 },
            new BillInfo { BillInfoID = 5, BillID = 3, ProductID = 5, Count = 2, Price = 10000 },
            new BillInfo { BillInfoID = 6, BillID = 3, ProductID = 1, Count = 1, Price = 45000 },
            new BillInfo { BillInfoID = 7, BillID = 4, ProductID = 2, Count = 2, Price = 10000 },
            new BillInfo { BillInfoID = 8, BillID = 5, ProductID = 3, Count = 1, Price = 45000 },
            new BillInfo { BillInfoID = 9, BillID = 6, ProductID = 4, Count = 2, Price = 10000 },
            new BillInfo { BillInfoID = 10, BillID = 7, ProductID = 5, Count = 1, Price = 45000 },
        };

        public Task<BillInfo> Add(BillInfo item)
        {
            item.BillInfoID = _items.Any() ? _items.Max(x => x.BillInfoID) + 1 : 1;

            _items.Add(item);

            Debug.WriteLine("ADD BILL INFO MOCK");
            return Task.FromResult(item);
        }

        public Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out int billInfoId)) return Task.FromResult(false);

            var item = _items.FirstOrDefault(x => x.BillInfoID == billInfoId);
            if (item == null) return Task.FromResult(false);

            _items.Remove(item);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<BillInfo>> GetAll()
        {
            return Task.FromResult<IEnumerable<BillInfo>>(_items);
        }

        public Task<BillInfo?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out int id)) return Task.FromResult<BillInfo?>(null);

            return Task.FromResult(_items.FirstOrDefault(x => x.BillInfoID == id));
        }

        public Task<IEnumerable<BillInfo>> GetByBillId(string billId)
        {
            if (!int.TryParse(billId, out int id)) return Task.FromResult<IEnumerable<BillInfo>>([]);

            return Task.FromResult<IEnumerable<BillInfo>>(_items.Where(x => x.BillID == id));
        }

        public Task<BillInfo?> GetItem(string billId, string productId)
        {
            if (!int.TryParse(billId, out int bId) || !int.TryParse(productId, out int pId))
                return Task.FromResult<BillInfo?>(null);

            return Task.FromResult(_items.FirstOrDefault(x => x.BillID == bId && x.ProductID == pId));
        }

        public Task<bool> Update(BillInfo item)
        {
            var existing = _items.FirstOrDefault(x => x.BillInfoID == item.BillInfoID);
            if (existing == null) return Task.FromResult(false);

            existing.Count = item.Count;
            existing.Price = item.Price;
            existing.Note = item.Note;

            return Task.FromResult(true);
        }
    }
}
