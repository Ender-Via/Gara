using System;

namespace WpfApp1.Models
{
    public class PaymentReceipt
    {
        public Guid Id { get; set; }
        public Guid RepairOrderId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal AmountReceived { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
