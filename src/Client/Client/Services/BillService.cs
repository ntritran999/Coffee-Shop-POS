using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Client.Services
{
    public class BillService
    {
        private IBillRepository _billRepository;
        private IBillInfoRepository _billInfoRepository;
        private IProductRepository _productRepository;

        public BillService(IBillInfoRepository billInfoRepository, IBillRepository billRepository)
        {
            _billInfoRepository = billInfoRepository;
            _billRepository = billRepository;
        }

        public async Task<bool> SaveNewBill(List<BillItem> items, bool isPaid)
        {
            var addedBill = await _billRepository.Add(new Bill()
            {
                TableID = null,
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

        public async Task<Bill?> GetOpenBillByTable(string tableId)
        {
            return await _billRepository.GetByTable(tableId);
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
    }
}
