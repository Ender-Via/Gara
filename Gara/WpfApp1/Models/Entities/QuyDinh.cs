using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 11. Quy Định Hệ Thống
    [Table("system_regulations")]
    public class QuyDinh : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("max_car_brands")]
        public int SoLuongHieuXeToiDa { get; set; }

        [Column("max_daily_vehicles")]
        public int SoXeTiepNhanToiDa { get; set; }

        [Column("max_parts")]
        public int SoLuongVatTuToiDa { get; set; }

        [Column("max_labors")]
        public int SoLuongTienCongToiDa { get; set; }

        public bool ChoPhepTiepNhan(int soLuongHienTai)
        {
            return soLuongHienTai < SoXeTiepNhanToiDa;
        }
    }
}
