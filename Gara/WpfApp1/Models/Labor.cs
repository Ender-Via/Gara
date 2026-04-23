using System;

namespace WpfApp1.Models
{
    public class Labor
    {
        public Guid Id { get; set; }
        public string LaborCode { get; set; } = string.Empty;
        public string LaborName { get; set; } = string.Empty;
        public decimal LaborFee { get; set; }
    }
}
