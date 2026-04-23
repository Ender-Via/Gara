using System;

namespace WpfApp1.Models
{
    public class ServiceReceipt
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime ReceptionDate { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
