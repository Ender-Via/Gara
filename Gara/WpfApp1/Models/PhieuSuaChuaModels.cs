using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.Models
{
    public partial class ChiTietSuaChua : ObservableObject
    {
        [ObservableProperty]
        private int _sTT;

        [ObservableProperty]
        private string _noiDung = string.Empty;

        [ObservableProperty]
        private string _vatTuPhuTung = string.Empty;

        [ObservableProperty]
        private VatTuItem? _vatTuDuocChon;

        [ObservableProperty]
        private int _soLuong = 1;

        [ObservableProperty]
        private double _donGia;

        [ObservableProperty]
        private double _tienCong;

        [ObservableProperty]
        private TienCongItem? _loaiTienCongDuocChon;

        public double ThanhTien => (SoLuong * DonGia) + TienCong;

        partial void OnSoLuongChanged(int value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnDonGiaChanged(double value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnTienCongChanged(double value) => OnPropertyChanged(nameof(ThanhTien));

        partial void OnVatTuDuocChonChanged(VatTuItem? value)
        {
            VatTuPhuTung = value?.Ten ?? string.Empty;
            DonGia = value?.DonGia ?? 0;
            OnPropertyChanged(nameof(ThanhTien));
        }

        partial void OnLoaiTienCongDuocChonChanged(TienCongItem? value)
        {
            TienCong = value?.SoTien ?? 0;
            OnPropertyChanged(nameof(ThanhTien));
        }
    }

    public class VatTuItem
    {
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public double DonGia { get; set; }
    }

    public class TienCongItem
    {
        public string Ma { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public double SoTien { get; set; }
    }

    public class PhieuSuaChuaRecord
    {
        public string BienSoXe { get; set; } = string.Empty;
        public DateTime NgaySuaChua { get; set; }
        public List<ChiTietSuaChuaRecord> DanhSachChiTiet { get; set; } = new();
        public double TongTien { get; set; }
    }

    public class ChiTietSuaChuaRecord
    {
        public int STT { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string VatTuPhuTung { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienCong { get; set; }
        public double ThanhTien { get; set; }
    }
}
