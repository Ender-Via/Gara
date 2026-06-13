using System;

namespace WpfApp1.Models.DTOs
{
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
