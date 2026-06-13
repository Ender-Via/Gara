using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Models.Entities
{
    // 5. Phiếu Sửa Chữa
    [Table("repair_orders")]
    public class PhieuSuaChua : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("repair_date")]
        public DateTime NgaySuaChua { get; set; }

        [Column("total_amount")]
        public decimal? TongTien { get; set; }

        // --- Khóa Ngoại ---
        [Column("service_receipt_id")]
        public string PhieuTiepNhanId { get; set; }

        [Reference(typeof(PhieuTiepNhan))]
        public PhieuTiepNhan PhieuTiepNhan { get; set; }

        public void TinhTongTien(IEnumerable<ChiTietPhieuSuaChua> dsChiTiet)
        {
            this.TongTien = dsChiTiet.Sum(ct => ct.ThanhTien ?? 0);
        }

        public bool KiemTraTrongKy(int thang, int nam)
        {
            return NgaySuaChua.Month == thang && NgaySuaChua.Year == nam;
        }

        /// <summary>
        /// Tạo danh sách các giao dịch xuất kho tương ứng với các vật tư trong phiếu sửa chữa
        /// </summary>
        public List<GiaoDichKho> TaoGiaoDichXuatKho(IEnumerable<ChiTietPhieuSuaChua> dsChiTiet)
        {
            return dsChiTiet
                .Where(ct => !string.IsNullOrEmpty(ct.VatTuPhuTungId))
                .Select(ct => new GiaoDichKho
                {
                    NgayGiaoDich = this.NgaySuaChua,
                    LoaiGiaoDich = "XUAT",
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGiaPhuTung,
                    VatTuPhuTungId = ct.VatTuPhuTungId
                }).ToList();
        }
    }
}
