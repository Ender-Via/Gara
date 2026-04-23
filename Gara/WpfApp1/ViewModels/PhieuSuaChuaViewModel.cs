using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public partial class PhieuSuaChuaViewModel : ObservableObject
    {
        private const string FilePath = "phieusuachua.json";
        private static readonly CultureInfo ViVnCulture = CultureInfo.GetCultureInfo("vi-VN");

        [ObservableProperty]
        private string _bienSoXe = string.Empty;

        [ObservableProperty]
        private DateTime _ngaySuaChua = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<ChiTietSuaChua> _danhSachChiTiet = new();

        [ObservableProperty]
        private ChiTietSuaChua? _dongDangChon;

        [ObservableProperty]
        private ObservableCollection<VatTuItem> _danhSachVatTu = new();

        [ObservableProperty]
        private ObservableCollection<TienCongItem> _danhSachTienCong = new();

        public string TongTienHienThi => $"Tổng tiền: {TinhTongTien().ToString("N0", ViVnCulture)} VNĐ";

        public PhieuSuaChuaViewModel()
        {
            KhoiTaoDanhMuc();
            DanhSachChiTiet.CollectionChanged += DanhSachChiTiet_CollectionChanged;
            DanhSachChiTiet.Add(new ChiTietSuaChua { SoLuong = 1 });
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

            var records = LoadFromFile();
            var phieu = new PhieuSuaChuaRecord
            {
                BienSoXe = BienSoXe,
                NgaySuaChua = NgaySuaChua,
                DanhSachChiTiet = DanhSachChiTiet.Select(x => new ChiTietSuaChuaRecord
                {
                    STT = x.STT,
                    NoiDung = x.NoiDung,
                    VatTuPhuTung = x.VatTuPhuTung,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    TienCong = x.TienCong,
                    ThanhTien = x.ThanhTien
                }).ToList(),
                TongTien = TinhTongTien()
            };

            records.Add(phieu);
            SaveToFile(records);

            MessageBox.Show("Lưu phiếu sửa chữa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

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
                foreach (ChiTietSuaChua item in e.NewItems)
                {
                    item.PropertyChanged += ChiTietSuaChua_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ChiTietSuaChua item in e.OldItems)
                {
                    item.PropertyChanged -= ChiTietSuaChua_PropertyChanged;
                }
            }

            CapNhatSTT();
            OnPropertyChanged(nameof(TongTienHienThi));
        }

        private void ChiTietSuaChua_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChiTietSuaChua.SoLuong) ||
                e.PropertyName == nameof(ChiTietSuaChua.DonGia) ||
                e.PropertyName == nameof(ChiTietSuaChua.TienCong) ||
                e.PropertyName == nameof(ChiTietSuaChua.ThanhTien))
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
            DanhSachChiTiet.Add(new ChiTietSuaChua { SoLuong = 1 });
            DongDangChon = null;
            OnPropertyChanged(nameof(TongTienHienThi));
        }

        private void KhoiTaoDanhMuc()
        {
            var vatTus = new List<VatTuItem>();
            for (int i = 1; i <= 200; i++)
            {
                vatTus.Add(new VatTuItem
                {
                    Ma = $"VT{i:D3}",
                    Ten = $"Vật tư {i}",
                    DonGia = 50000 + (i * 1000)
                });
            }

            var tienCongs = new List<TienCongItem>();
            for (int i = 1; i <= 100; i++)
            {
                tienCongs.Add(new TienCongItem
                {
                    Ma = $"TC{i:D3}",
                    Ten = $"Tiền công loại {i}",
                    SoTien = 80000 + (i * 2000)
                });
            }

            DanhSachVatTu = new ObservableCollection<VatTuItem>(vatTus);
            DanhSachTienCong = new ObservableCollection<TienCongItem>(tienCongs);
        }

        private void CapNhatSTT()
        {
            for (int i = 0; i < DanhSachChiTiet.Count; i++)
            {
                DanhSachChiTiet[i].STT = i + 1;
            }
        }

        private static List<PhieuSuaChuaRecord> LoadFromFile()
        {
            if (!File.Exists(FilePath))
            {
                return new List<PhieuSuaChuaRecord>();
            }

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<PhieuSuaChuaRecord>>(json)
                   ?? new List<PhieuSuaChuaRecord>();
        }

        private static void SaveToFile(List<PhieuSuaChuaRecord> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }

}