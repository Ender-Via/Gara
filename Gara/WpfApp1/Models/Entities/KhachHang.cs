using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace WpfApp1.Models.Entities
{
    // 2. Khách Hàng
    [Table("customers")]
    public class KhachHang : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("full_name")]
        public string HoTen { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public string DienThoai { get; set; }

        [Column("address")]
        public string DiaChi { get; set; }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại (ví dụ: tối thiểu 10 chữ số)
        /// </summary>
        public bool KiemTraDinhDangSDT()
        {
            if (string.IsNullOrWhiteSpace(DienThoai)) return false;
            return DienThoai.Length >= 10 && long.TryParse(DienThoai, out _);
        }

        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        public void CapNhatThongTin(string hoTen, string dienThoai, string diaChi)
        {
            this.HoTen = hoTen;
            this.DienThoai = dienThoai;
            this.DiaChi = diaChi;
        }
    }
}
