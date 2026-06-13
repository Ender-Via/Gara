using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1.Models.Entities
{
    // 8. Chi Tiết Phiếu Sửa Chữa
    [Table("repair_order_details")]
    public class ChiTietPhieuSuaChua : BaseModel, INotifyPropertyChanged
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("content")]
        public string NoiDung { get; set; }

        private decimal? _soLuong;
        private decimal? _donGiaPhuTung;
        private decimal? _chiPhiTienCong;
        private decimal? _thanhTien;

        [Column("quantity")]
        public decimal? SoLuong
        {
            get => _soLuong;
            set { _soLuong = value; OnPropertyChanged(); }
        }

        [Column("unit_price")]
        public decimal? DonGiaPhuTung
        {
            get => _donGiaPhuTung;
            set { _donGiaPhuTung = value; OnPropertyChanged(); }
        }

        [Column("labor_fee")]
        public decimal? ChiPhiTienCong
        {
            get => _chiPhiTienCong;
            set { _chiPhiTienCong = value; OnPropertyChanged(); }
        }

        [Column("line_total")]
        public decimal? ThanhTien
        {
            get => _thanhTien;
            set { _thanhTien = value; OnPropertyChanged(); }
        }

        [Column("repair_order_id")]
        public string PhieuSuaChuaId { get; set; }

        [Reference(typeof(PhieuSuaChua))]
        public PhieuSuaChua PhieuSuaChua { get; set; }

        [Column("part_id")]
        public string VatTuPhuTungId { get; set; }

        [Reference(typeof(VatTuPhuTung))]
        public VatTuPhuTung VatTuPhuTung { get; set; }

        [Column("labor_id")]
        public string TienCongId { get; set; }

        [Reference(typeof(TienCong))]
        public TienCong TienCong { get; set; }

        public void TinhThanhTienChiTiet()
        {
            this.ThanhTien = (SoLuong ?? 0) * (DonGiaPhuTung ?? 0) + (ChiPhiTienCong ?? 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
