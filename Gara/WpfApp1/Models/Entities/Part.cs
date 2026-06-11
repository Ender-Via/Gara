using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 6. Phụ Tùng
    [Table("parts")]
    public class Part : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("part_name")]
        public string PartName { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("unit_price")]
        public decimal? UnitPrice { get; set; }

        [Column("stock_quantity")]
        public decimal? StockQuantity { get; set; }
    }
}
