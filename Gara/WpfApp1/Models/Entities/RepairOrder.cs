using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 5. Lệnh Sửa Chữa
    [Table("repair_orders")]
    public class RepairOrder : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("repair_date")]
        public DateTime RepairDate { get; set; }

        [Column("total_amount")]
        public decimal? TotalAmount { get; set; }

        // --- Khóa Ngoại ---
        [Column("service_receipt_id")]
        public string ServiceReceiptId { get; set; }

        [Reference(typeof(ServiceReceipt))]
        public ServiceReceipt ServiceReceipt { get; set; }
    }
}
