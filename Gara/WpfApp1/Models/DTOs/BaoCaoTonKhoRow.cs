using System;

namespace WpfApp1.Models.DTOs
{
    public class BaoCaoTonKhoRow
    {
        public int STT { get; set; }
        public string VatTuPhuTung { get; set; }
        public decimal TonDau { get; set; }
        public decimal PhatSinh { get; set; }

        // Thuộc tính để hiển thị có dấu + hoặc -
        public string PhatSinhDisplay => PhatSinh > 0 ? $"+{PhatSinh:N0}" : PhatSinh.ToString("N0");

        public decimal TonCuoi { get; set; }
    }
}
