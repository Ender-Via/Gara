using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 4. Phiếu Tiếp Nhận Dịch Vụ
    [Table("service_receipts")]
    public class ServiceReceipt : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("reception_date")]
        public DateTime ReceptionDate { get; set; }

        // --- Khóa Ngoại ---
        [Column("vehicle_id")]
        public string VehicleId { get; set; }

        [Reference(typeof(Vehicle))]
        public Vehicle Vehicle { get; set; }
    }
}
