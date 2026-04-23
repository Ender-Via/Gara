using System;

namespace WpfApp1.Models
{
    public class SystemRegulation
    {
        public Guid Id { get; set; }
        public string RegulationKey { get; set; } = string.Empty;
        public string RegulationValue { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
