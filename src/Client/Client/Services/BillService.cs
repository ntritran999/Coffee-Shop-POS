using Client.Models;
using Client.Repositories;
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
    }
}
