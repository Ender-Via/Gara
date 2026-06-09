using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Models;

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
        public PhieuSuaChuaWindow()
        {
            InitializeComponent();
            this.Loaded += Window_loaded;
        }
        private async void Window_loaded(object sender, RoutedEventArgs e)
        {
            if (_danhSachChiTiet == null)
            {
                _danhSachChiTiet = new ObservableCollection<RepairOrderDetail>();
            }

            dgvChiTiet.ItemsSource = _danhSachChiTiet;
            try
            {
                // Tải xe
                var _vehicles = await App.DB._client.From<Vehicle>().Get();
                if (_vehicles != null && _vehicles.Models != null)
                {
                    var danhsachxe = _vehicles.Models;
                    var danhsachbienso = danhsachxe.Select(x => x.LicensePlate).ToList();
                    txtBienSo.ItemsSource = danhsachbienso;
                }

                // Tải phụ tùng
                var _parts = await App.DB._client.From<Part>().Get();
                if (_parts != null && _parts.Models != null)
                {
                    _danhSachPhuTung = new ObservableCollection<Part>(_parts.Models);
                    colVatTu.ItemsSource = _danhSachPhuTung;
                }

                // Tải tiền công
                var _labors = await App.DB._client.From<Labor>().Get();
                if (_labors != null && _labors.Models != null)
                {
                    _danhSachTienCong = new ObservableCollection<Labor>(_labors.Models);
                    colTienCong.ItemsSource = _danhSachTienCong;
                }
            }
            catch (Exception ex)
            {
                // Có lỗi DB thì nó hiện lên đây cho m biết, bảng vẫn gõ chữ bình thường
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
            decimal total = 0;
            foreach (var item in _danhSachChiTiet)
            {
                total += item.LineTotal ?? 0;
            }
            txtTotalAmount.Text = total.ToString("N0"); 
        }
        private async void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Kiểm tra UI xem nhập đủ chưa
                if (txtBienSo.SelectedValue == null)
                {
                    MessageBox.Show("Chưa chọn biển số xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime ngaySua = dpNgayTiepNhan.SelectedDate ?? DateTime.Now;

                bool isSuccess = await App.DB.LuuPhieuSuaChuaAsync(ngaySua, _danhSachChiTiet);

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
