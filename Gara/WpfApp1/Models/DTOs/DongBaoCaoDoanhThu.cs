using System;

namespace WpfApp1.Models.DTOs
{
    public class BaoCaoDoanhThuRow
    {
        public int STT { get; set; }
        public string HieuXe { get; set; }
        public int SoLuotSua { get; set; }
        public decimal ThanhTien { get; set; }
        public double TiLe { get; set; }

        public void TinhTiLe(decimal tongDoanhThu)
        {
            if (tongDoanhThu > 0)
                TiLe = Math.Round((double)(ThanhTien / tongDoanhThu) * 100, 2);
            else
                TiLe = 0;
        }
    }
}
