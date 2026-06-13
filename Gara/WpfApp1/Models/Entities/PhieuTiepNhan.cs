using Postgrest.Attributes;
using Postgrest.Models;
using System;
using WpfApp1.Models.DTOs;

namespace WpfApp1.Models.Entities
{
    // 4. Phiếu Tiếp Nhận
    [Table("service_receipts")]
    public class PhieuTiepNhan : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("reception_date")]
        public DateTime NgayTiepNhan { get; set; }

        // --- Khóa Ngoại ---
        [Column("vehicle_id")]
        public string XeId { get; set; }

        [Reference(typeof(Xe))]
        public Xe Xe { get; set; }

        public bool KiemTraNgayTiepNhan(DateTime ngay)
        {
            return NgayTiepNhan.Date == ngay.Date;
        }

        public RecentReceiptDTO TaoDTOHienThi()
        {
            return new RecentReceiptDTO
            {
                TenKhach = Xe?.KhachHang?.HoTen ?? "Không rõ",
                BienSo = Xe?.BienSo ?? "Không rõ",
                HieuXe = Xe?.HieuXe?.TenHieuXe ?? string.Empty,
                ThoiGian = NgayTiepNhan.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            };
        }
    }
}
