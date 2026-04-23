using System;

namespace WpfApp1.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid CarBrandId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
