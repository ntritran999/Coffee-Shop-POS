using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class OrderDetail
    {
        public int BillID { get; set; }
        public DateTime DateCheckIn { get; set; }
        public int? TableID { get; set; }
        public string TableName { get; set; } = "";
        public List<BillItem> BillItems { get; set; } = [];

        public int TotalPrice = 0;
        public int Discount { get; set; } = 0;
    }
}
