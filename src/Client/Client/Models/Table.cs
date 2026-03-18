using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public string TableName { get; set; }
        public int Status { get; set; } // 0: Trống, 1: Có người, 2: Đang dọn dẹp
    }
}
