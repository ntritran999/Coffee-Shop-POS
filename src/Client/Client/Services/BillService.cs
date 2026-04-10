using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class BillService
    {
        private IBillRepository _billRepository;
        private IBillInfoRepository _billInfoRepository;

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
    }
}
