using System;

namespace WpfApp1.Models
{
    public class Part
    {
        public Guid Id { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal StockQuantity { get; set; }
    }
}
