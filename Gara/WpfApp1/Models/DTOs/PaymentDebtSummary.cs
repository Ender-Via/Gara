using System;

namespace WpfApp1.Models.DTOs
{
    public class PaymentDebtSummary
    {
        public Entities.Vehicle? Vehicle { get; set; }
        public Entities.Customer? Customer { get; set; }
        public decimal TotalRepairAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal CurrentDebt { get; set; }
        public string LatestRepairOrderId { get; set; } = string.Empty;
    }
}
