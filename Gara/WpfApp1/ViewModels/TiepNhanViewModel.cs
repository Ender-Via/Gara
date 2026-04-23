using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp1.ViewModels
{
    public partial class TiepNhanViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _tenChuXe;

        [ObservableProperty]
        private string _bienSo;

        [ObservableProperty]
        private string _dienThoai;

        [ObservableProperty]
        private string _diaChi;

        [ObservableProperty]
        private DateTime _ngayTiepNhan = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<string> _danhSachHieuXe;

        [ObservableProperty]
        private string _hieuXeDuocChon;

        public TiepNhanViewModel()
        {
            LoadHieuXe();
        }

        private void LoadHieuXe()
        {
            DanhSachHieuXe = new ObservableCollection<string>
            {
                "Toyota", "Honda", "Suzuki", "Ford", "Kia", "Hyundai", "Mazda", "BMW", "Mercedes", "Nissan"
            };
            HieuXeDuocChon = DanhSachHieuXe[0];
        }

        [RelayCommand]
        private void Luu()
        {
            MessageBox.Show("Đã lưu thông tin tiếp nhận!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Dong(Window window)
        {
            window?.Close();
        }
    }
}