using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models.Entities;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for PhieuSuaChuaWindow.xaml
    /// </summary>
    public partial class PhieuSuaChuaWindow : Window
    {
        private ObservableCollection<RepairOrderDetail> _danhSachChiTiet = new ObservableCollection<RepairOrderDetail>();
        private ObservableCollection<Part> _danhSachPhuTung = new ObservableCollection<Part>();
        private ObservableCollection<Labor> _danhSachTienCong = new ObservableCollection<Labor>();
        private readonly PhieuSuaChuaViewModel _viewModel;

        public PhieuSuaChuaWindow()
        {
            InitializeComponent();
            _viewModel = new PhieuSuaChuaViewModel();
            this.Loaded += Window_loaded;
        }

        private async void Window_loaded(object sender, RoutedEventArgs e)
        {
            dgvChiTiet.ItemsSource = _danhSachChiTiet;
            try
            {
                // Tải biển số xe
                var danhsachbienso = await _viewModel.GetLicensePlatesAsync();
                txtBienSo.ItemsSource = danhsachbienso;

                // Tải phụ tùng
                var parts = await _viewModel.GetPartsAsync();
                _danhSachPhuTung = new ObservableCollection<Part>(parts);
                colVatTu.ItemsSource = _danhSachPhuTung;

                // Tải tiền công
                var labors = await _viewModel.GetLaborsAsync();
                _danhSachTienCong = new ObservableCollection<Labor>(labors);
                colTienCong.ItemsSource = _danhSachTienCong;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu DB: " + ex.Message, "Thông báo");
            }
        }

        private void dgvChiTiet_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var row = e.Row.Item as RepairOrderDetail;
            if (row == null) return;

            if(e.Column.Header.ToString() == "Vật tư phụ tùng")
            {
                var combo = e.EditingElement as ComboBox;
                var partId = combo?.SelectedValue?.ToString();
                var selectedPart = _danhSachPhuTung.FirstOrDefault(p => p.Id == partId);

                if (selectedPart != null)
                {
                    row.UnitPrice = selectedPart.UnitPrice;
                }
            }
            else if (e.Column.Header.ToString() == "Loại tiền công")
            {
                var combo = e.EditingElement as ComboBox;
                var laborId = combo?.SelectedValue?.ToString();
                var selectedLabor = _danhSachTienCong.FirstOrDefault(l => l.Id == laborId);

                if (selectedLabor != null)
                {
                    row.LaborFee = selectedLabor.LaborFee;
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                decimal sl = row.Quantity ?? 0;
                decimal gia = row.UnitPrice ?? 0;
                decimal cong = row.LaborFee ?? 0;

                row.LineTotal = (sl * gia) + cong;

                TinhTongTienPhieu();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void TinhTongTienPhieu()
        {
            decimal total = _viewModel.CalculateTotal(_danhSachChiTiet);
            txtTotalAmount.Text = total.ToString("N0"); 
        }

        private async void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtBienSo.SelectedValue == null)
                {
                    MessageBox.Show("Chưa chọn biển số xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime ngaySua = dpNgayTiepNhan.SelectedDate ?? DateTime.Now;

                bool isSuccess = await _viewModel.LuuPhieuSuaChuaAsync(ngaySua, _danhSachChiTiet);

                if (isSuccess)
                {
                    MessageBox.Show("Lưu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
