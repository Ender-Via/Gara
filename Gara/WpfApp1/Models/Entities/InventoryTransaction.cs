using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 10. Giao Dịch Kho (Nhập / Xuất)
    [Table("inventory_transactions")]
    public class InventoryTransaction : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Column("transaction_type")]
        public string TransactionType { get; set; }

        [Column("quantity")]
        public decimal? Quantity { get; set; }

        [Column("unit_price")]
        public decimal? UnitPrice { get; set; }

        // --- Khóa Ngoại ---
        [Column("part_id")]
        public string PartId { get; set; }

        [Reference(typeof(Part))]
        public Part Part { get; set; }
    }
}
