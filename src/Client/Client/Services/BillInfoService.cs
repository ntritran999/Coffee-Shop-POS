using Client.Models;
using Client.Repositories;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class BillInfoService
    {
        public IBillRepository _billRepo = new MockBillRepository();
        public IBillInfoRepository _billInfoRepo = new MockBillInfoRepository();
        public IProductRepository _productRepo = new MockProductRepoitory();
        public BillInfoService() { }
        public async Task<List<TopSellingProductItem>> GetTopSellingProductsAsync(int top = 5)
        {
            var bills = await _billRepo.GetAll();
            var paidBills = bills.Where(b => b.Status == 1);

            var allBillInfos = new List<BillInfo>();
            foreach (var bill in paidBills)
            {
                var infos = await _billInfoRepo.GetByBillId(bill.BillID.ToString());
                allBillInfos.AddRange(infos);
            }

            var grouped = allBillInfos
                .GroupBy(bi => bi.ProductID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(bi => bi.Count)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(top)
                .ToList();

            var products = await _productRepo.GetAll();

            var result = grouped
                .Select(x =>
                {
                    var product = products.FirstOrDefault(p => p.ProductID == x.ProductId);
                    return product != null
                        ? new TopSellingProductItem { product = product, totalSold = x.TotalSold }
                        : null;
                })
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();

            return result;
        }

        public async Task<List<RecentOrderItem>> GetRecentOrderDetailsAsync(int top = 3)
        {
            var bills = await _billRepo.GetAll();
            var recentBills = bills.OrderByDescending(b => b.DateCheckIn).Take(top);

            var result = new List<RecentOrderItem>();

            foreach (var bill in recentBills)
            {
                var items = await _billInfoRepo.GetByBillId(bill.BillID.ToString());
                result.Add(new RecentOrderItem
                {
                    BillID = bill.BillID,
                    DateCheckIn = bill.DateCheckIn,
                    TableID = bill.TableID,
                    Status = bill.Status,
                    TotalAmount = bill.TotalAmount,
                    Items = items.ToList()
                });
            }

            return result;
        }
    }
}
