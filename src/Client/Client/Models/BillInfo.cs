using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class BillInfo
    {
        public int BillInfoID { get; set; }
        public int BillID { get; set; }
        public int ProductID { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public string Note { get; set; } // Ít đường, không đá...
    }
}
