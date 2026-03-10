using System;
using System.Collections.Generic;
using GrapQLServer.Models;

namespace GrapQLServer.Data
{
    public class BillRepository
    {
        private readonly List<Bill> _bills = new List<Bill>
        {
            new Bill { ID = "B001", DateCheckIn = DateTime.Now.AddHours(-3), DateCheckOut = DateTime.Now.AddHours(-1), TableID = "T1", Status = "Paid", TotalAmount = 120.50m, Discount = 10m },
            new Bill { ID = "B002", DateCheckIn = DateTime.Now.AddHours(-2), DateCheckOut = null, TableID = "T2", Status = "Open", TotalAmount = 60.00m, Discount = 0m },
        };

        public IReadOnlyList<Bill> GetBills() => _bills.AsReadOnly();
    }
}
