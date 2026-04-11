using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class OrderLine
    {
        public int BillID { get; set; }

        public DateTime DateCheckIn { get; set; }
        public int? TableID { get; set; }
        public string TableName { get; set; } = "";

        public int TotalPrice { get; set; }
        public int Discount { get; set; }

        public int StatusRaw { get; set; }
        public string Status { get; set; } = "";
    }
}
