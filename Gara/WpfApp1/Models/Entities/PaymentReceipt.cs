using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 9. Phiếu Thu Tiền
    [Table("payment_receipts")]
    public class PaymentReceipt : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("receipt_date")]
        public DateTime ReceiptDate { get; set; }

        [Column("amount_received")]
        public decimal? AmountReceived { get; set; }

        [Column("note")]
        public string Note { get; set; }

        // --- Khóa Ngoại ---
        [Column("repair_order_id")]
        public string RepairOrderId { get; set; }

        [Reference(typeof(RepairOrder))]
        public RepairOrder RepairOrder { get; set; }
    }
}
