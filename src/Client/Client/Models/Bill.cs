using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Bill
    {
        public int BillID { get; set; }
        public DateTime DateCheckIn { get; set; }

        // Dùng DateTime? vì có thể hóa đơn chưa thanh toán nên chưa có giờ ra
        public DateTime? DateCheckOut { get; set; }

        // Dùng int? vì cho phép Null nếu khách mua mang về
        public int? TableID { get; set; }

        public int Status { get; set; } // 0: Chưa thanh toán, 1: Đã thanh toán
        public double TotalAmount { get; set; }
        public double Discount { get; set; }

        public List<BillInfo> BillInfo { get; set; } = new();
    }
}
