using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 11. Quy Định Hệ Thống
    [Table("system_regulations")]
    public class SystemRegulation : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("max_car_brands")]
        public int MaxCarBrands { get; set; }

        [Column("max_daily_vehicles")]
        public int MaxDailyVehicles { get; set; }

        [Column("max_parts")]
        public int MaxParts { get; set; }

        [Column("max_labors")]
        public int MaxLabors { get; set; }
    }
}
