using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp1.ViewModels
{
    public partial class TiepNhanViewModel : ObservableObject
    {
        private const int SoXeToiDaMoiNgay = 30;

        [ObservableProperty]
        private string _tenChuXe = string.Empty;

        [ObservableProperty]
        private string _bienSo = string.Empty;

        [ObservableProperty]
        private string _dienThoai = string.Empty;

        [ObservableProperty]
        private string _diaChi = string.Empty;

        [ObservableProperty]
        private DateTime _ngayTiepNhan = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<string> _danhSachHieuXe = new();

        [ObservableProperty]
        private string _hieuXeDuocChon = string.Empty;

        [ObservableProperty]
        private string _soXeDaTiepNhanTrongNgayHienThi = string.Empty;

        public TiepNhanViewModel()
        {
            LoadHieuXe();
            CapNhatSoXeDaTiepNhanTrongNgay();
        }

        partial void OnNgayTiepNhanChanged(DateTime value)
        {
            CapNhatSoXeDaTiepNhanTrongNgay();
        }

        private void LoadHieuXe()
        {
            DanhSachHieuXe = LayDanhSachHieuXeTuDatabase();
            HieuXeDuocChon = DanhSachHieuXe[0];
        }

        [RelayCommand]
        private void Luu()
        {
            if (!ThongTinHopLe())
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin: tên chủ xe, biển số, điện thoại, địa chỉ, hiệu xe.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var bienSoChuanHoa = BienSo.Trim().ToUpperInvariant();
            var ngayTiepNhan = NgayTiepNhan.Date;

            if (!KiemTraGioiHanTiepNhanTrongNgay(ngayTiepNhan))
            {
                MessageBox.Show("Mỗi ngày chỉ tiếp nhận tối đa 30 xe.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (KiemTraXeDaTiepNhanTrongNgay(bienSoChuanHoa, ngayTiepNhan))
            {
                MessageBox.Show("Xe này đã được tiếp nhận trong ngày đã chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            LuuTiepNhanVaoDatabase();

            MessageBox.Show("Đã ghi nhận yêu cầu tiếp nhận (chưa kết nối database).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LamMoiThongTin();
        }

        [RelayCommand]
        private void Dong(Window window)
        {
            window?.Close();
        }

        private bool ThongTinHopLe()
        {
            return !string.IsNullOrWhiteSpace(TenChuXe)
                   && !string.IsNullOrWhiteSpace(BienSo)
                   && !string.IsNullOrWhiteSpace(DienThoai)
                   && !string.IsNullOrWhiteSpace(DiaChi)
                   && !string.IsNullOrWhiteSpace(HieuXeDuocChon)
                   && DanhSachHieuXe.Contains(HieuXeDuocChon);
        }

        private void LamMoiThongTin()
        {
            TenChuXe = string.Empty;
            BienSo = string.Empty;
            DienThoai = string.Empty;
            DiaChi = string.Empty;
            HieuXeDuocChon = DanhSachHieuXe.FirstOrDefault() ?? string.Empty;
            NgayTiepNhan = DateTime.Now;
        }

        private void CapNhatSoXeDaTiepNhanTrongNgay()
        {
            var soLuong = LaySoXeDaTiepNhanTrongNgay(NgayTiepNhan.Date);
            SoXeDaTiepNhanTrongNgayHienThi = $"Số xe đã tiếp nhận trong ngày: {soLuong}/{SoXeToiDaMoiNgay}";
        }

        private ObservableCollection<string> LayDanhSachHieuXeTuDatabase()
        {
            MessageBox.Show("[DB Placeholder] TODO: Query bảng car_brands để lấy danh sách hiệu xe.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return new ObservableCollection<string>
            {
                "Toyota", "Honda", "Suzuki", "Ford", "Kia", "Hyundai", "Mazda", "BMW", "Mercedes", "Nissan"
            };
        }

        private bool KiemTraGioiHanTiepNhanTrongNgay(DateTime ngayTiepNhan)
        {
            MessageBox.Show("[DB Placeholder] TODO: Query service_receipts theo reception_date để kiểm tra giới hạn 30 xe/ngày.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        private bool KiemTraXeDaTiepNhanTrongNgay(string bienSo, DateTime ngayTiepNhan)
        {
            MessageBox.Show("[DB Placeholder] TODO: Query vehicles + service_receipts để kiểm tra xe đã tiếp nhận trong ngày.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        private void LuuTiepNhanVaoDatabase()
        {
            MessageBox.Show("[DB Placeholder] TODO: Lưu vào customers, vehicles, service_receipts.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private int LaySoXeDaTiepNhanTrongNgay(DateTime ngayTiepNhan)
        {
            MessageBox.Show("[DB Placeholder] TODO: Query số lượng service_receipts trong ngày.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return 0;
        }
    }
}