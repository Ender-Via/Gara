using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public partial class PhieuSuaChuaChiTietItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _sTT;

        [ObservableProperty]
        private string _noiDung = string.Empty;

        [ObservableProperty]
        private string _vatTuPhuTung = string.Empty;

        [ObservableProperty]
        private Part? _vatTuDuocChon;

        [ObservableProperty]
        private int _soLuong = 1;

        [ObservableProperty]
        private double _donGia;

        [ObservableProperty]
        private double _tienCong;

        [ObservableProperty]
        private Labor? _loaiTienCongDuocChon;

        public double ThanhTien => (SoLuong * DonGia) + TienCong;

        partial void OnSoLuongChanged(int value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnDonGiaChanged(double value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnTienCongChanged(double value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnVatTuDuocChonChanged(Part? value)
        {
            VatTuPhuTung = value?.PartName ?? string.Empty;
            DonGia = value == null ? 0 : (double)value.UnitPrice;
            OnPropertyChanged(nameof(ThanhTien));
        }

        partial void OnLoaiTienCongDuocChonChanged(Labor? value)
        {
            TienCong = value == null ? 0 : (double)value.LaborFee;
            OnPropertyChanged(nameof(ThanhTien));
        }
    }
}
