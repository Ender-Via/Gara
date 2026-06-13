using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 9. Phiếu Thu Tiền
    [Table("payment_receipts")]
    public class PhieuThuTien : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("receipt_date")]
        public DateTime NgayThuTien { get; set; }

        [Column("amount_received")]
        public decimal? SoTienThu { get; set; }

        [Column("note")]
        public string GhiChu { get; set; }

        // --- Khóa Ngoại ---
        [Column("repair_order_id")]
        public string PhieuSuaChuaId { get; set; }

        [Reference(typeof(PhieuSuaChua))]
        public PhieuSuaChua PhieuSuaChua { get; set; }

        public bool KiemTraThanhToanDu(decimal tongTienSuaChua)
        {
            return (SoTienThu ?? 0) >= tongTienSuaChua;
        }
    }
}
