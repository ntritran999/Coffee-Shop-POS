using System;

namespace GrapQLServer.Models
{
    public class Bill
    {
        public string? ID { get; set; }
        public DateTime? DateCheckIn { get; set; }
        public DateTime? DateCheckOut { get; set; }
        public string? TableID { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
    }
}
