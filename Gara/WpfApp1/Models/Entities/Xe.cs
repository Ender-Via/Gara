using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 3. Xe
    [Table("vehicles")]
    public class Xe : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("license_plate")]
        public string BienSo { get; set; }

        // --- Khóa Ngoại (Chuyển thành Association theo Quy tắc 2) ---
        [Column("customer_id")]
        public string KhachHangId { get; set; }

        [Reference(typeof(KhachHang))]
        public KhachHang KhachHang { get; set; }

        [Column("car_brand_id")]
        public string HieuXeId { get; set; }

        [Reference(typeof(HieuXe))]
        public HieuXe HieuXe { get; set; }

        public void GanChuXe(KhachHang khachHang)
        {
            this.KhachHang = khachHang;
            this.KhachHangId = khachHang?.Id;
        }

        public void GanHieuXe(HieuXe hieuXe)
        {
            this.HieuXe = hieuXe;
            this.HieuXeId = hieuXe?.Id;
        }

        public bool KiemTraBienSo(string bienSo)
        {
            return string.Equals(this.BienSo, bienSo, System.StringComparison.OrdinalIgnoreCase);
        }

        public decimal TinhTienNo(IEnumerable<PhieuSuaChua> tatCaPhieuSua, IEnumerable<PhieuThuTien> tatCaPhieuThu)
        {
            var phieuSuaCuaXe = tatCaPhieuSua.Where(p => p.PhieuTiepNhan?.XeId == this.Id || p.PhieuTiepNhanId == this.Id).ToList(); // Logic join
            // Đơn giản hóa: Xe -> PhieuTiepNhan -> PhieuSuaChua
            decimal tongSua = phieuSuaCuaXe.Sum(p => p.TongTien ?? 0);
            var idsPhieuSua = phieuSuaCuaXe.Select(p => p.Id).ToList();
            decimal tongThu = tatCaPhieuThu.Where(p => idsPhieuSua.Contains(p.PhieuSuaChuaId)).Sum(p => p.SoTienThu ?? 0);
            return tongSua - tongThu;
        }
    }
}
