using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 1. Hiệu Xe
    [Table("car_brands")]
    public class HieuXe : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("brand_name")]
        public string TenHieuXe { get; set; }

        public bool KiemTraTenTrung(string tenMoi)
        {
            return string.Equals(this.TenHieuXe, tenMoi, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
