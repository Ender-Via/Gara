using System;
using WpfApp1.Models.Entities;

namespace WpfApp1.Models.DTOs
{
    public class PaymentDebtSummary
    {
        public Xe? Vehicle { get; set; }
        public KhachHang? Customer { get; set; }
        public decimal TotalRepairAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal CurrentDebt { get; set; }
        public string LatestRepairOrderId { get; set; } = string.Empty;
    }
}
