using Postgrest.Attributes;
using Postgrest.Models;

namespace WpfApp1.Models.Entities
{
    public enum UserRole
    {
        Admin,
        LeTan,
        KyThuatVien,
        KeToan
    }

    [Table("staff")]
    public class NhanVien : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("name")]
        public string HoTen { get; set; }

        [Column("role")]
        public string RoleString { get; set; }

        public UserRole Role
        {
            get
            {
                return RoleString switch
                {
                    "Admin" => UserRole.Admin,
                    "LeTan" => UserRole.LeTan,
                    "KyThuatVien" => UserRole.KyThuatVien,
                    "KeToan" => UserRole.KeToan,
                    _ => UserRole.LeTan // Default
                };
            }
        }
    }
}
