using System;

namespace WpfApp1.Models
{
    public class RepairOrder
    {
        public Guid Id { get; set; }
        public Guid ServiceReceiptId { get; set; }
        public DateTime RepairDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
