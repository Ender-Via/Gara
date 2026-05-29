using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class PaymentDebtSummary
    {
        public Vehicle? Vehicle { get; set; }
        public Customer? Customer { get; set; }
        public decimal TotalRepairAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal CurrentDebt { get; set; }
        public string LatestRepairOrderId { get; set; } = string.Empty;
    }

    public class RecentPaymentReceiptRow
    {
        public string MaPhieu { get; set; } = string.Empty;
        public string ChuXe { get; set; } = string.Empty;
        public string BienSo { get; set; } = string.Empty;
        public string NgayThu { get; set; } = string.Empty;
        public string SoTien { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "THÀNH CÔNG";
    }
}
