using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    // 2. Khách Hàng
    [Table("customers")]
    public class Customer : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        public string Address { get; set; }
    }

    // 3. Phương Tiện (Xe)
    [Table("vehicles")]
    public class Vehicle : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("license_plate")]
        public string LicensePlate { get; set; }

        [Column("note")]
        public string Note { get; set; }

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

    // 4. Phiếu Tiếp Nhận Dịch Vụ
    [Table("service_receipts")]
    public class ServiceReceipt : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("reception_date")]
        public DateTime ReceptionDate { get; set; }

        [Column("note")]
        public string Note { get; set; }

        // --- Khóa Ngoại ---
        [Column("vehicle_id")]
        public string VehicleId { get; set; }

        [Reference(typeof(Vehicle))]
        public Vehicle Vehicle { get; set; }
    }

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

        [Column("note")]
        public string Note { get; set; }

        // --- Khóa Ngoại ---
        [Column("service_receipt_id")]
        public string ServiceReceiptId { get; set; }

        [Reference(typeof(ServiceReceipt))]
        public ServiceReceipt ServiceReceipt { get; set; }
    }

    // 6. Phụ Tùng
    [Table("parts")]
    public class Part : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("part_code")]
        public string PartCode { get; set; }

        [Column("part_name")]
        public string PartName { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("unit_price")]
        public decimal? UnitPrice { get; set; }

        [Column("stock_quantity")]
        public decimal? StockQuantity { get; set; }
    }

    // 7. Nhân Công
    [Table("labors")]
    public class Labor : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("labor_code")]
        public string LaborCode { get; set; }

        [Column("labor_name")]
        public string LaborName { get; set; }

        [Column("labor_fee")]
        public decimal? LaborFee { get; set; }
    }

    // 8. Chi Tiết Lệnh Sửa Chữa
    [Table("repair_order_details")]
    public class RepairOrderDetail : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("line_no")]
        public int? LineNo { get; set; }

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

        [Column("note")]
        public string Note { get; set; }

        // --- Khóa Ngoại ---
        [Column("part_id")]
        public string PartId { get; set; }

        [Reference(typeof(Part))]
        public Part Part { get; set; }
    }

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
