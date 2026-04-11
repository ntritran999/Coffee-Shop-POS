using Client.Models;
using Client.Repositories;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class BillService
    {
        private IBillRepository _billRepository;
        private IBillInfoRepository _billInfoRepository;
        private ITableRepository _tableRepository;
        private IProductRepository _productRepository;

        public BillService(IBillInfoRepository billInfoRepository, IBillRepository billRepository,
                           ITableRepository tableRepository, IProductRepository productRepository)
        {
            _billInfoRepository = billInfoRepository;
            _billRepository = billRepository;
            _tableRepository = tableRepository;
            _productRepository = productRepository;
        }

        public async Task<List<OrderLine>> GetOrders()
        {
            var orderLines = new List<OrderLine>();
            var orders = await _billRepository.GetAll();
            if (orders == null) return orderLines;

            var tableTasks = orders.Select(order =>
                _tableRepository.GetById(order?.TableID.ToString() ?? "")
            ).ToList();

            Table?[] tables = await Task.WhenAll(tableTasks);

            for (int i = 0; i < orders.Count(); i++)
            {
                string tableName = tables[i]?.TableName ?? "Mang đi";
                var order = orders.ElementAt(i);
                orderLines.Add(new()
                {
                    BillID = order.BillID,
                    DateCheckIn = order.DateCheckIn,
                    TableID = tables[i]?.TableID,
                    TableName = tableName,
                    TotalPrice = (int)order.TotalAmount,
                    Discount = (int)order.Discount,
                    StatusRaw = order.Status,
                    Status = order.Status switch
                    {
                        0 => "Chưa thanh toán",
                        1 => "Đã thanh toán",
                        2 => "Huỷ",
                        _ => "Không xác định"
                    }
                });

            }
            return orderLines;
        }

        public async Task<OrderDetail> GetOrderDetail(OrderLine orderLine)
        {
            var billInfos = await _billInfoRepository.GetByBillId(orderLine.BillID.ToString());
            if (billInfos == null) return new();

            var productTasks = billInfos.Select(info => _productRepository.GetById(info.ProductID.ToString())).ToList();

            Product?[] products = await Task.WhenAll(productTasks);
            if (products.Contains(null)) return new();

            var billItems = new List<BillItem>();
            for (int i = 0; i < billInfos.Count(); i++)
            {
                var billInfo = billInfos.ElementAt(i);
                billItems.Add(new BillItem(products[i], billInfo.Count)
                {
                    Notes = billInfo.Note,
                });
            }

            return new()
            {
                BillID = orderLine.BillID,
                DateCheckIn = orderLine.DateCheckIn,
                TableID = orderLine.TableID,
                TableName = orderLine.TableName,
                BillItems = billItems,
                TotalPrice = (int)billInfos.Sum(info => info.Price * info.Count),
                Discount = orderLine.Discount,
            };
        }

        public async Task<bool> SaveNewBill(List<BillItem> items, Table table, bool isPaid)
        {
            var addedBill = await _billRepository.Add(new Bill()
            {
                TableID = table.TableID != -1 ? table.TableID : null,
                Discount = 0,
            }); 

            if (addedBill == null)
            {
                Debug.WriteLine("No added bill");
                return false;
            }

            if (isPaid)
            {
                var res = await _billRepository.Update(new Bill()
                {
                    BillID = addedBill.BillID,
                    TableID = addedBill.TableID,
                    Discount = addedBill.Discount,
                    Status = 1,
                    DateCheckOut = DateTime.Now,
                });

                if (!res)
                {
                    Debug.WriteLine("No update bill");
                    return false; 
                }
            }

            List<Task<BillInfo>> tasks = [];
            foreach (var item in items)
            {
                tasks.Add(_billInfoRepository.Add(new BillInfo()
                {
                    BillID = addedBill.BillID,
                    ProductID = item.Detail!.ProductID,
                    Count = item.Count,
                    Price = item.Detail!.Price,
                    Note = item.Notes
                }));
            }

            BillInfo[] infos = await Task.WhenAll(tasks);
            foreach (var info in infos)
            {
                if (info == null)
                {
                    Debug.WriteLine("No add billitem");
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> UpdateOrder(OrderDetail detail, int status)
        {
            var res = await _billRepository.Update(new Bill()
            {
                BillID = detail.BillID,
                TableID = detail.TableID,
                Discount = detail.Discount,
                Status = status,
                DateCheckOut = DateTime.Now,
            });

            if (!res)
            {
                Debug.WriteLine("No update bill");
                return false;
            }

            return true;
        }

        public async Task<bool> PayOrder(OrderDetail detail)
        {
            int statusPaid = 1;
            return await UpdateOrder(detail, statusPaid);
        }

        public async Task<bool> CancelOrder(OrderDetail detail)
        {
            int statusCanceled = 2;
            return await UpdateOrder(detail, statusCanceled);
        }

        public async Task<Bill?> GetOpenBillByTable(string tableId)
        {
            return (await _billRepository.GetByTable(tableId)).FirstOrDefault();
        }

        public async Task AddProduct(string billId, string productId, int quantity)
        {
            var product = await _productRepository.GetById(productId);
            if (product == null) return;

            var existing = await _billInfoRepository.GetItem(billId, productId);

            if (existing != null)
            {
                existing.Count += quantity;
                await _billInfoRepository.Update(existing);
            }
            else
            {
                var item = new BillInfo
                {
                    BillID = int.Parse(billId),
                    ProductID = int.Parse(productId),
                    Count = quantity,
                    Price = product.Price
                };

                await _billInfoRepository.Add(item);
            }

            await UpdateTotal(billId);
        }

        public async Task RemoveProduct(string billId, string productId)
        {
            var existing = await _billInfoRepository.GetItem(billId, productId);
            if (existing == null) return;

            if (existing.Count > 1)
            {
                existing.Count--;
                await _billInfoRepository.Update(existing);
            }
            else
            {
                await _billInfoRepository.Delete(existing.BillInfoID.ToString());
            }

            await UpdateTotal(billId);
        }

        public async Task<IEnumerable<BillInfo>> GetBillItems(string billId)
        {
            return await _billInfoRepository.GetByBillId(billId);
        }

        public async Task<double> CalculateTotal(string billId)
        {
            var items = await _billInfoRepository.GetByBillId(billId);
            var sum = items.Sum(i => i.Count * i.Price);

            var bill = await _billRepository.GetById(billId);
            if (bill == null) return sum;

            return sum - bill.Discount;
        }

        public async Task<bool> Checkout(string billId)
        {
            var bill = await _billRepository.GetById(billId);
            if (bill == null) return false;

            bill.Status = 1;
            bill.DateCheckOut = DateTime.Now;
            bill.TotalAmount = await CalculateTotal(billId);

            return await _billRepository.Update(bill);
        }

        private async Task UpdateTotal(string billId)
        {
            var bill = await _billRepository.GetById(billId);
            if (bill == null) return;

            bill.TotalAmount = await CalculateTotal(billId);
            await _billRepository.Update(bill);
        }

        public async Task<List<TopSellingProductItem>> GetTopSellingProductsAsync(int top = 5)
        {
            try
            {
                var bills = await _billRepository.GetAll();
                var paidBills = bills.Where(b => b.Status == 1).ToList();

                var allBillInfos = new List<BillInfo>();
                foreach (var bill in paidBills)
                {
                    var infos = await _billInfoRepository.GetByBillId(bill.BillID.ToString());
                    allBillInfos.AddRange(infos);
                }
                Debug.WriteLine($"Total bill infos: {allBillInfos.Count}");
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

                var products = await _productRepository.GetAll();

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
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<List<RecentOrderItem>> GetRecentOrderDetailsAsync(int top = 3)
        {
            try
            {
                var bills = await _billRepository.GetAll();
                var recentBills = bills.OrderByDescending(b => b.DateCheckIn).Take(top).ToList();

                var result = new List<RecentOrderItem>();

                foreach (var bill in recentBills)
                {
                    var items = await _billInfoRepository.GetByBillId(bill.BillID.ToString());
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
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<List<Bill>> GetRevenueByDateAsync(DateTime fromDate, DateTime toDate)
        {
            return (await _billRepository.GetByDate(fromDate, toDate)).ToList();
        }
    }
}
