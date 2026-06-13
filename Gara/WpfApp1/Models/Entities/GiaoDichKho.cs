using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 10. Giao Dịch Kho
    [Table("inventory_transactions")]
    public class GiaoDichKho : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("transaction_date")]
        public DateTime NgayGiaoDich { get; set; }

        [Column("transaction_type")]
        public string LoaiGiaoDich { get; set; }

        [Column("quantity")]
        public decimal? SoLuong { get; set; }

        [Column("unit_price")]
        public decimal? DonGia { get; set; }

        // --- Khóa Ngoại ---
        [Column("part_id")]
        public string VatTuPhuTungId { get; set; }

        [Reference(typeof(VatTuPhuTung))]
        public VatTuPhuTung VatTuPhuTung { get; set; }

        public decimal LayGiaTriBienDong()
        {
            return (SoLuong ?? 0) * (DonGia ?? 0);
        }
    }
}
