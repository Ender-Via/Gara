using System;

namespace WpfApp1.Models
{
    public class RepairOrderDetail
    {
        public Guid Id { get; set; }
        public Guid RepairOrderId { get; set; }
        public short LineNo { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? PartId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid? LaborId { get; set; }
        public decimal LaborFee { get; set; }
        public decimal LineTotal { get; set; }
    }
}
