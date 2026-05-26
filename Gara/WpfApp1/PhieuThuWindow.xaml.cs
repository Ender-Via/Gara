using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class PhieuThuWindow : Window
    {
        private readonly SupabaseService _service;
        private readonly ObservableCollection<RecentPaymentReceiptRow> _recentPayments = new();
        private decimal _currentDebt;
        private string _lastRepairOrderId = string.Empty;
        private string _loadedLicensePlate = string.Empty;

        public PhieuThuWindow()
        {
            InitializeComponent();

            _service = App.DB ?? new SupabaseService();
            dgGiaoDichGanDay.ItemsSource = _recentPayments;

            txtMaPhieu.IsReadOnly = true;
            dpNgayThu.SelectedDate = DateTime.Now;

            Loaded += PhieuThuWindow_Loaded;
            txtBienSo.LostFocus += TxtBienSo_LostFocus;
        }

        private async void PhieuThuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_service._client == null)
                    await _service.InitializeAsync();

                await GenerateAutoCodeAsync();
                await LoadRecentPaymentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải phiếu thu tiền: " + ex.Message);
            }
        }

        private async Task GenerateAutoCodeAsync()
        {
            try
            {
                txtMaPhieu.Text = await _service.GetNextPaymentCodeAsync();
            }
            catch (Exception ex)
            {
                txtMaPhieu.Text = "ERROR";
                MessageBox.Show("Lỗi tự động tạo mã phiếu: " + ex.Message);
            }
        }

        private async Task LoadRecentPaymentsAsync()
        {
            _recentPayments.Clear();

            var rows = await _service.GetRecentPaymentReceiptsAsync();
            foreach (var row in rows)
            {
                _recentPayments.Add(row);
            }

            txtEmptyRecentPayments.Visibility = rows.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async void TxtBienSo_LostFocus(object sender, RoutedEventArgs e)
        {
            var bienSo = txtBienSo.Text.Trim();
            if (string.IsNullOrWhiteSpace(bienSo))
                return;

            await LoadVehiclePaymentContextAsync(bienSo, showSuccessMessage: true);
        }

        private async Task<bool> LoadVehiclePaymentContextAsync(string bienSo, bool showSuccessMessage)
        {
            try
            {
                var summary = await _service.GetVehiclePaymentSummaryAsync(bienSo);
                if (summary.Vehicle == null)
                {
                    ResetDebtSummary();
                    MessageBox.Show("Không tìm thấy xe này trong hệ thống!");
                    return false;
                }

                _currentDebt = summary.CurrentDebt;
                _lastRepairOrderId = summary.LatestRepairOrderId;
                _loadedLicensePlate = bienSo;

                txtTenKH.Text = summary.Customer?.FullName ?? string.Empty;
                txtSDT.Text = summary.Customer?.Phone ?? string.Empty;
                txtEmail.Text = summary.Customer?.Email ?? string.Empty;

                UpdateDebtSummary(summary.TotalRepairAmount, summary.TotalPaidAmount, summary.CurrentDebt);

                if (showSuccessMessage)
                {
                    MessageBox.Show($"Thông tin xe hợp lệ. Số tiền đang nợ: {_currentDebt:N0} VNĐ");
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin công nợ: " + ex.Message);
                return false;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var bienSo = txtBienSo.Text.Trim();
            if (string.IsNullOrWhiteSpace(bienSo) || string.IsNullOrWhiteSpace(txtSoTien.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ biển số và số tiền thu!");
                return;
            }

            if (!TryParseMoney(txtSoTien.Text, out var soTienThu) || soTienThu <= 0)
            {
                MessageBox.Show("Số tiền thu không hợp lệ!");
                return;
            }

            if (!string.Equals(_loadedLicensePlate, bienSo, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(_lastRepairOrderId))
            {
                var loaded = await LoadVehiclePaymentContextAsync(bienSo, showSuccessMessage: false);
                if (!loaded)
                    return;
            }

            if (string.IsNullOrWhiteSpace(_lastRepairOrderId))
            {
                MessageBox.Show("Xe này chưa có phiếu sửa chữa để lập phiếu thu.");
                return;
            }

            if (soTienThu > _currentDebt)
            {
                MessageBox.Show($"Vi phạm quy định 4: Số tiền thu ({soTienThu:N0}) không được vượt quá số tiền nợ ({_currentDebt:N0})!");
                return;
            }

            var originalButtonContent = btnSave.Content;
            btnSave.IsEnabled = false;
            btnSave.Content = "Đang xử lý...";

            try
            {
                var ghiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text)
                    ? $"Phiếu thu {txtMaPhieu.Text}"
                    : txtGhiChu.Text.Trim();
                var ngayThu = dpNgayThu.SelectedDate ?? DateTime.Now;

                await _service.CreatePaymentReceiptAsync(bienSo, soTienThu, ngayThu, ghiChu);

                MessageBox.Show("Lưu phiếu thu tiền thành công!");
                txtSoTien.Text = string.Empty;

                await LoadVehiclePaymentContextAsync(bienSo, showSuccessMessage: false);
                await GenerateAutoCodeAsync();
                await LoadRecentPaymentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
            finally
            {
                btnSave.Content = originalButtonContent;
                btnSave.IsEnabled = true;
            }
        }

        private static bool TryParseMoney(string value, out decimal amount)
        {
            var normalized = value
                .Replace("VNĐ", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("VND", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();

            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out amount)
                || decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount)
                || decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.CurrentCulture, out amount);
        }

        private void UpdateDebtSummary(decimal totalRepairAmount, decimal totalPaidAmount, decimal currentDebt)
        {
            txtTongTienSua.Text = $"{totalRepairAmount:N0} VND";
            txtDaThanhToan.Text = $"{totalPaidAmount:N0} VND";
            txtConNo.Text = $"{currentDebt:N0} VND";
        }

        private void ResetDebtSummary()
        {
            _currentDebt = 0;
            _lastRepairOrderId = string.Empty;
            _loadedLicensePlate = string.Empty;
            UpdateDebtSummary(0, 0, 0);
        }

        private void txtMaPhieu_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
