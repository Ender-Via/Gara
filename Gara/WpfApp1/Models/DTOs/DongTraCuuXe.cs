using System;

namespace WpfApp1.Models.DTOs
{
    // DTO dùng để hiển thị tra cứu xe (không map DB)
    public class TraCuuXeRow
    {
        public int STT { get; set; }
        public string BienSo { get; set; }
        public string HieuXe { get; set; }
        public string ChuXe { get; set; }
        public decimal TienNo { get; set; }
    }
}
