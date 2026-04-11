using System;
using System.Collections.Generic;

namespace Client.Models
{
    public class TopSellingProductItem
    {
        public Product product { get; set; } = default!;
        public int totalSold { get; set; }
    }

    public class RecentOrderItem
    {
        public int BillID { get; set; }
        public DateTime DateCheckIn { get; set; }
        public int? TableID { get; set; }
        public int Status { get; set; }

        public string StatusText => Status == 1 ? "Đã thanh toán" : "Chưa thanh toán";
        public double TotalAmount { get; set; }
        public List<BillInfo> Items { get; set; } = new();
    }
}
