using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    // 7. Nhân Công
    [Table("labors")]
    public class Labor : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("labor_name")]
        public string LaborName { get; set; }

        [Column("labor_fee")]
        public decimal? LaborFee { get; set; }
    }
}
