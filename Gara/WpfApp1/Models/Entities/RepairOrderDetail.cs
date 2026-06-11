using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1.Models.Entities
{
    // 8. Chi Tiết Lệnh Sửa Chữa
    [Table("repair_order_details")]
    public class RepairOrderDetail : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("content")]
        public string Content { get; set; }

        private decimal? _quantity;
        private decimal? _unitPrice;
        private decimal? _laborFee;
        private decimal? _lineTotal;

        [Column("quantity")]
        public decimal? Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        [Column("unit_price")]
        public decimal? UnitPrice
        {
            get => _unitPrice;
            set { _unitPrice = value; OnPropertyChanged(); }
        }

        [Column("labor_fee")]
        public decimal? LaborFee
        {
            get => _laborFee;
            set { _laborFee = value; OnPropertyChanged(); }
        }

        [Column("line_total")]
        public decimal? LineTotal
        {
            get => _lineTotal;
            set { _lineTotal = value; OnPropertyChanged(); }
        }

        [Column("repair_order_id")]
        public string RepairOrderId { get; set; }

        [Reference(typeof(RepairOrder))]
        public RepairOrder RepairOrder { get; set; }

        [Column("part_id")]
        public string PartId { get; set; }

        [Reference(typeof(Part))]
        public Part Part { get; set; }

        [Column("labor_id")]
        public string LaborId { get; set; }

        [Reference(typeof(Labor))]
        public Labor Labor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
