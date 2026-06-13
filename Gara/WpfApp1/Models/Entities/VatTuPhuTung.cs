using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.Models.DTOs;

namespace WpfApp1.Models.Entities
{
    // 6. Vật Tư Phụ Tùng
    [Table("parts")]
    public class VatTuPhuTung : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("part_name")]
        public string TenPhuTung { get; set; }

        [Column("unit")]
        public string DonViTinh { get; set; }

        [Column("unit_price")]
        public decimal? DonGia { get; set; }

        [Column("stock_quantity")]
        public decimal? SoLuongTon { get; set; }

        public void CapNhatSoLuong(decimal bienDong, string loaiGiaoDich)
        {
            if (loaiGiaoDich == "NHAP")
                SoLuongTon = (SoLuongTon ?? 0) + bienDong;
            else if (loaiGiaoDich == "XUAT")
                SoLuongTon = (SoLuongTon ?? 0) - bienDong;
        }

        public BaoCaoTonKhoRow TaoDongBaoCaoTonKho(IEnumerable<GiaoDichKho> tatCaGiaoDich, int thang, int nam, int stt)
        {
            // 1. Thiết lập các mốc thời gian chuẩn UTC
            DateTime startDate = new DateTime(nam, thang, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = startDate.AddMonths(1);

            // 2. Lọc giao dịch của riêng phụ tùng này và chuẩn hóa múi giờ
            var partTransactions = tatCaGiaoDich
                .Where(t => t.VatTuPhuTungId == this.Id)
                .Select(t => {
                    if (t.NgayGiaoDich.Kind == DateTimeKind.Unspecified)
                        t.NgayGiaoDich = DateTime.SpecifyKind(t.NgayGiaoDich, DateTimeKind.Utc);
                    else
                        t.NgayGiaoDich = t.NgayGiaoDich.ToUniversalTime();
                    return t;
                }).ToList();

            // A. TỒN ĐẦU: Tổng (Nhập - Xuất) TRƯỚC ngày startDate
            decimal tonDauNhap = partTransactions
                .Where(t => t.NgayGiaoDich < startDate && t.LoaiGiaoDich == "NHAP")
                .Sum(t => t.SoLuong ?? 0);
            decimal tonDauXuat = partTransactions
                .Where(t => t.NgayGiaoDich < startDate && t.LoaiGiaoDich == "XUAT")
                .Sum(t => t.SoLuong ?? 0);
            decimal tonDau = tonDauNhap - tonDauXuat;

            // B. PHÁT SINH TRONG THÁNG: Tổng biến động (Nhập - Xuất) TRONG tháng
            decimal phatSinhNhap = partTransactions
                .Where(t => t.NgayGiaoDich >= startDate && t.NgayGiaoDich < endDate && t.LoaiGiaoDich == "NHAP")
                .Sum(t => t.SoLuong ?? 0);
            decimal phatSinhXuat = partTransactions
                .Where(t => t.NgayGiaoDich >= startDate && t.NgayGiaoDich < endDate && t.LoaiGiaoDich == "XUAT")
                .Sum(t => t.SoLuong ?? 0);

            // Cột Phát sinh hiển thị sự thay đổi ròng (Nhập thêm - Tiêu thụ)
            decimal phatSinh = phatSinhNhap - phatSinhXuat;

            // C. TỒN CUỐI: Tồn đầu + Phát sinh
            decimal tonCuoi = tonDau + phatSinh;

            return new BaoCaoTonKhoRow
            {
                STT = stt,
                VatTuPhuTung = this.TenPhuTung,
                TonDau = tonDau,
                PhatSinh = phatSinh,
                TonCuoi = tonCuoi
            };
        }
    }
}
