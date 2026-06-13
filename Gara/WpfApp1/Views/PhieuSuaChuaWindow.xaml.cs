using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class PhieuSuaChuaWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ChiTietPhieuSuaChua> _danhSachChiTiet = new ObservableCollection<ChiTietPhieuSuaChua>();
        private ObservableCollection<VatTuPhuTung> _danhSachPhuTung = new ObservableCollection<VatTuPhuTung>();
        private ObservableCollection<TienCong> _danhSachTienCong = new ObservableCollection<TienCong>();
        private readonly PhieuSuaChuaViewModel _viewModel;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<VatTuPhuTung> DanhSachPhuTung
        {
            get => _danhSachPhuTung;
            set { _danhSachPhuTung = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TienCong> DanhSachTienCong
        {
            get => _danhSachTienCong;
            set { _danhSachTienCong = value; OnPropertyChanged(); }
        }

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
                DanhSachPhuTung = new ObservableCollection<VatTuPhuTung>(parts);
                colVatTu.ItemsSource = DanhSachPhuTung;

                // Tải tiền công
                var labors = await _viewModel.GetLaborsAsync();
                DanhSachTienCong = new ObservableCollection<TienCong>(labors);
                colTienCong.ItemsSource = DanhSachTienCong;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu DB: " + ex.Message, "Thông báo");
            }
        }

        private void dgvChiTiet_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var row = e.Row.Item as ChiTietPhieuSuaChua;
            if (row == null) return;

            if(e.Column.Header.ToString() == "Vật tư phụ tùng")
            {
                var combo = e.EditingElement as ComboBox;
                var partId = combo?.SelectedValue?.ToString();
                var selectedPart = DanhSachPhuTung.FirstOrDefault(p => p.Id == partId);

                if (selectedPart != null)
                {
                    row.DonGiaPhuTung = selectedPart.DonGia;
                }
            }
            else if (e.Column.Header.ToString() == "Loại tiền công")
            {
                var combo = e.EditingElement as ComboBox;
                var laborId = combo?.SelectedValue?.ToString();
                var selectedLabor = DanhSachTienCong.FirstOrDefault(l => l.Id == laborId);

                if (selectedLabor != null)
                {
                    row.ChiPhiTienCong = selectedLabor.ChiPhi;
                }
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                row.TinhThanhTienChiTiet();
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
                string bienSo = txtBienSo.SelectedValue?.ToString() ?? "";

                bool isSuccess = await _viewModel.LuuPhieuSuaChuaAsync(bienSo, ngaySua, _danhSachChiTiet);

                if (isSuccess)
                {
                    MessageBox.Show("Lưu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form for next input instead of closing/navigating
                    txtBienSo.SelectedIndex = -1;
                    dpNgayTiepNhan.SelectedDate = DateTime.Now;
                    _danhSachChiTiet.Clear();
                    txtTotalAmount.Text = "0";
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
