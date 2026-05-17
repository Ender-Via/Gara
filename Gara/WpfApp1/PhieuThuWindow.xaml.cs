using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class PhieuThuWindow : Window
    {
        private SupabaseService _service;
        private decimal _currentDebt = 0; // Lưu nợ hiện tại để check QĐ4
        private string _lastRepairOrderId = ""; // Lưu ID lệnh sửa chữa gần nhất để gán phiếu thu

        public PhieuThuWindow()
        {
            InitializeComponent();
            _service = new SupabaseService();
            

            txtMaPhieu.IsReadOnly = true;
            txtMaPhieu.Background = System.Windows.Media.Brushes.LightGray;

            Loaded += async (s, e) => {
                await _service.InitializeAsync();
                await GenerateAutoCode(); 
            };

            // Gán ngày mặc định là hôm nay
            dpNgayThu.SelectedDate = DateTime.Now;

            // Đăng ký sự kiện khi nhập xong biển số xe
            txtBienSo.LostFocus += TxtBienSo_LostFocus;
        }


        private async Task GenerateAutoCode()
        {
            try
            {
                string nextCode = await _service.GetNextPaymentCodeAsync();
                txtMaPhieu.Text = nextCode;
            }
            catch (Exception ex)
            {
                txtMaPhieu.Text = "ERROR";
                MessageBox.Show("Lỗi tự động tạo mã phiếu: " + ex.Message);
            }
        }
        private async void TxtBienSo_LostFocus(object sender, RoutedEventArgs e)
        {
            string bienSo = txtBienSo.Text.Trim();
            if (string.IsNullOrEmpty(bienSo)) return;

            try
            {
                // Gọi service lấy data nợ
                var (vehicle, debt) = await _service.GetVehicleDebtAsync(bienSo);

                if (vehicle != null)
                {
                    _currentDebt = debt;

                    // Lấy thông tin khách hàng (phải lấy thêm từ bảng Customer)
                    var customerRes = await _service._client.From<Customer>()
                        .Filter("id", Postgrest.Constants.Operator.Equals, vehicle.CustomerId)
                        .Get();
                    var customer = customerRes.Models.FirstOrDefault();

                    if (customer != null)
                    {
                        txtTenKH.Text = customer.FullName;
                        txtSDT.Text = customer.Phone;
                        txtEmail.Text = customer.Email;
                    }

                    
                    var lastOrderRes = await _service._client.From<RepairOrder>()
                        .Order("repair_date", Postgrest.Constants.Ordering.Descending)
                        .Get(); 

                    _lastRepairOrderId = lastOrderRes.Models.FirstOrDefault()?.Id ?? "";

                    MessageBox.Show($"Thông tin xe hợp lệ. Số tiền đang nợ: {_currentDebt:N0} VNĐ");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy xe này trong hệ thống!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin: " + ex.Message);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validate dữ liệu đầu vào
            if (string.IsNullOrEmpty(txtBienSo.Text) || string.IsNullOrEmpty(txtSoTien.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Biển số và Số tiền thu!");
                return;
            }

            if (!decimal.TryParse(txtSoTien.Text, out decimal soTienThu))
            {
                MessageBox.Show("Số tiền thu không hợp lệ!");
                return;
            }

            // 2. Kiểm tra QĐ4: Số tiền thu không vượt quá tiền nợ
            if (soTienThu > _currentDebt)
            {
                MessageBox.Show($"Vi phạm quy định 4: Số tiền thu ({soTienThu:N0}) không được vượt quá số tiền nợ ({_currentDebt:N0})!");
                return;
            }

            try
            {
                string ghiChu = new TextRange(txtGhiChu.Document.ContentStart, txtGhiChu.Document.ContentEnd).Text.Trim();
                DateTime ngayThu = dpNgayThu.SelectedDate ?? DateTime.Now;

                // 3. Gọi Service lưu vào Database
                bool result = await _service.LuuPhieuThuAsync(_lastRepairOrderId, soTienThu, ngayThu, ghiChu);

                if (result)
                {
                    MessageBox.Show("Lưu phiếu thu tiền thành công!");
                    this.Close(); // Hoặc xóa trắng form
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message);
            }
        }

        // Dummy event theo XAML 
        private void txtMaPhieu_TextChanged(object sender, TextChangedEventArgs e) { }
    }
}