using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 1. Hãng Xe
    [Table("car_brands")]
    public class CarBrand : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("brand_name")]
        public string BrandName { get; set; }
    }
}
