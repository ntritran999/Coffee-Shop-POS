using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Unit { get; set; } // Đơn vị tính: 1: Ly, 2: Cốc, 3: Đĩa...
        public int CategoryID { get; set; }
        public string Image { get; set; } // URL hình ảnh
    }
}
