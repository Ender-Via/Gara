using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 3. Phương Tiện (Xe)
    [Table("vehicles")]
    public class Vehicle : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("license_plate")]
        public string LicensePlate { get; set; }

        // --- Khóa Ngoại ---
        [Column("customer_id")]
        public string CustomerId { get; set; }

        [Reference(typeof(Customer))]
        public Customer Customer { get; set; }

        [Column("car_brand_id")]
        public string CarBrandId { get; set; }

        [Reference(typeof(CarBrand))]
        public CarBrand CarBrand { get; set; }
    }
}
