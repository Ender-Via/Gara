using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 7. Tiền Công
    [Table("labors")]
    public class TienCong : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("labor_name")]
        public string TenNoiDung { get; set; }

        [Column("labor_fee")]
        public decimal? ChiPhi { get; set; }

        public void CapNhatDonGia(decimal chiPhiMoi)
        {
            this.ChiPhi = chiPhiMoi;
        }
    }
}
