using System;

namespace WpfApp1.Models
{
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public Guid PartId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
