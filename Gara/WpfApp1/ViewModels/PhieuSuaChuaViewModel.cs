using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public partial class PhieuSuaChuaViewModel : ObservableObject
    {
        private static readonly CultureInfo ViVnCulture = CultureInfo.GetCultureInfo("vi-VN");

        [ObservableProperty]
        private string _bienSoXe = string.Empty;

        [ObservableProperty]
        private DateTime _ngaySuaChua = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<PhieuSuaChuaChiTietItemViewModel> _danhSachChiTiet = new();

        [ObservableProperty]
        private PhieuSuaChuaChiTietItemViewModel? _dongDangChon;

        [ObservableProperty]
        private ObservableCollection<Part> _danhSachVatTu = new();

        [ObservableProperty]
        private ObservableCollection<Labor> _danhSachTienCong = new();

        public string TongTienHienThi => $"Tổng tiền: {TinhTongTien().ToString("N0", ViVnCulture)} VNĐ";

        public PhieuSuaChuaViewModel()
        {
            KhoiTaoDanhMuc();
            DanhSachChiTiet.CollectionChanged += DanhSachChiTiet_CollectionChanged;
            DanhSachChiTiet.Add(new PhieuSuaChuaChiTietItemViewModel { SoLuong = 1 });
        }

        [RelayCommand]
        private void XoaDong()
        {
            if (DongDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DanhSachChiTiet.Remove(DongDangChon);
            CapNhatSTT();
            OnPropertyChanged(nameof(TongTienHienThi));
        }

        [RelayCommand]
        private void InPhieu()
        {
            MessageBox.Show("Chức năng in phiếu sẽ được bổ sung sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void LuuPhieu()
        {
            if (string.IsNullOrWhiteSpace(BienSoXe))
            {
                MessageBox.Show("Vui lòng nhập biển số xe.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DanhSachChiTiet.Any() || !ChiTietHopLe())
            {
                MessageBox.Show("Vui lòng nhập chi tiết hợp lệ: chọn vật tư, loại tiền công và số lượng > 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LuuPhieuSuaChuaVaoDatabase();
            LuuChiTietPhieuSuaChuaVaoDatabase();

            MessageBox.Show("Đã ghi nhận phiếu sửa chữa (chưa kết nối database).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            LamMoiPhieu();
        }

        private double TinhTongTien()
        {
            return DanhSachChiTiet.Sum(x => x.ThanhTien);
        }

        private void DanhSachChiTiet_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PhieuSuaChuaChiTietItemViewModel item in e.NewItems)
                {
                    item.PropertyChanged += ChiTietSuaChua_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (PhieuSuaChuaChiTietItemViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= ChiTietSuaChua_PropertyChanged;
                }
            }

            CapNhatSTT();
            OnPropertyChanged(nameof(TongTienHienThi));
        }

        private void ChiTietSuaChua_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PhieuSuaChuaChiTietItemViewModel.SoLuong) ||
                e.PropertyName == nameof(PhieuSuaChuaChiTietItemViewModel.DonGia) ||
                e.PropertyName == nameof(PhieuSuaChuaChiTietItemViewModel.TienCong) ||
                e.PropertyName == nameof(PhieuSuaChuaChiTietItemViewModel.ThanhTien))
            {
                OnPropertyChanged(nameof(TongTienHienThi));
            }
        }

        private bool ChiTietHopLe()
        {
            return DanhSachChiTiet.All(x =>
                x.SoLuong > 0 &&
                x.VatTuDuocChon != null &&
                x.LoaiTienCongDuocChon != null);
        }

        private void LamMoiPhieu()
        {
            BienSoXe = string.Empty;
            NgaySuaChua = DateTime.Now;
            DanhSachChiTiet.Clear();
            DanhSachChiTiet.Add(new PhieuSuaChuaChiTietItemViewModel { SoLuong = 1 });
            DongDangChon = null;
            OnPropertyChanged(nameof(TongTienHienThi));
        }

        private void KhoiTaoDanhMuc()
        {
            var vatTus = new List<Part>();
            for (int i = 1; i <= 200; i++)
            {
                vatTus.Add(new Part
                {
                    Id = Guid.NewGuid(),
                    PartCode = $"VT{i:D3}",
                    PartName = $"Vật tư {i}",
                    Unit = "Cái",
                    UnitPrice = 50000 + (i * 1000),
                    StockQuantity = 100
                });
            }

            var tienCongs = new List<Labor>();
            for (int i = 1; i <= 100; i++)
            {
                tienCongs.Add(new Labor
                {
                    Id = Guid.NewGuid(),
                    LaborCode = $"TC{i:D3}",
                    LaborName = $"Tiền công loại {i}",
                    LaborFee = 80000 + (i * 2000)
                });
            }

            DanhSachVatTu = new ObservableCollection<Part>(vatTus);
            DanhSachTienCong = new ObservableCollection<Labor>(tienCongs);
        }

        private void CapNhatSTT()
        {
            for (int i = 0; i < DanhSachChiTiet.Count; i++)
            {
                DanhSachChiTiet[i].STT = i + 1;
            }
        }

        private void LuuPhieuSuaChuaVaoDatabase()
        {
            MessageBox.Show("[DB Placeholder] TODO: Insert vào bảng repair_orders.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LuuChiTietPhieuSuaChuaVaoDatabase()
        {
            MessageBox.Show("[DB Placeholder] TODO: Insert vào bảng repair_order_details.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }

}