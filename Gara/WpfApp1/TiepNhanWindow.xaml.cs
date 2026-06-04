using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class TiepNhanWindow : Window
    {
        public TiepNhanWindow()
        {
            InitializeComponent();

            
            this.Loaded += TiepNhanWindow_Loaded;
        }

        private async void TiepNhanWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await App.DB._client.From<CarBrand>().Get();
                var danhSachHieuXe = response.Models;

                cmbHieuXe.ItemsSource = danhSachHieuXe;
                cmbHieuXe.DisplayMemberPath = "BrandName"; 
                cmbHieuXe.SelectedValuePath = "Id";        

                if (danhSachHieuXe.Any())
                {
                    cmbHieuXe.SelectedIndex = 0;
                }

                dpNgayTiepNhan.SelectedDate = DateTime.Now;

                // Load dynamic dashboard stats
                var stats = await App.DB.GetDashboardStatsAsync();
                
                // Cập nhật text quy định (top bên trái)
                txtQuyDinhToiDa.Text = stats.MaxDailyVehicles.ToString();
                txtQuyDinhHieuXe.Text = danhSachHieuXe.Count.ToString();
                
                // Cập nhật widget trạng thái trong ngày
                txtLuotXeHienTai.Text = stats.LuotXeTrongNgay.ToString();
                txtLuotXeToiDa.Text = $"/ {stats.MaxDailyVehicles} xe";
                
                double phanTram = (double)stats.LuotXeTrongNgay / stats.MaxDailyVehicles * 100;
                txtPhanTramCongSuat.Text = $"{Math.Round(phanTram)}% công suất";
                
                // Progress bar
                double progressWidth = 310; // width of parent border (need to set it in XAML or assume ~310. E.g., it was 110/some_value)
                rectProgressBar.Width = (phanTram / 100.0) * progressWidth; 
                
                int xeConLai = stats.MaxDailyVehicles - stats.LuotXeTrongNgay;
                if(xeConLai < 0) xeConLai = 0;
                
                txtTrangThaiVanHanh.Text = phanTram >= 80 ? "Gara đang ở mức vận hành cao. Bạn có thể tiếp nhận thêm tối đa" :
                                          (phanTram >= 50 ? "Gara đang ở mức vận hành trung bình. Bạn có thể tiếp nhận thêm tối đa" : 
                                                            "Gara đang ở mức vận hành thấp. Bạn có thể tiếp nhận thêm tối đa");
                txtXeConLai.Text = $" {xeConLai} xe ";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
     

        private async void btnLuu_Click(object sender, RoutedEventArgs e)
        {

            string tenKhach = txtTenChuXe.Text.Trim();
            string sdt = txtDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string bienSo = txtBienSo.Text.Trim();

            string tenHieuXe = cmbHieuXe.Text;

            DateTime selectedDate = dpNgayTiepNhan.SelectedDate ?? DateTime.Now;
            // Lưu giờ phút giây hiện tại theo giờ local để có thời gian khác nhau mỗi lần tiếp nhận
            DateTime ngayTiepNhan = selectedDate.Date + DateTime.Now.TimeOfDay;

            if (string.IsNullOrEmpty(tenKhach) || string.IsNullOrEmpty(bienSo))
            {
                MessageBox.Show("Thiếu tên chủ xe hoặc biển số xe", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSuccess = await App.DB.LuuTiepNhanXeAsync(tenKhach, sdt, diaChi, bienSo, tenHieuXe, ngayTiepNhan);

            if (isSuccess)
            {
                MessageBox.Show("Đã tiếp nhận xe!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                txtTenChuXe.Clear();
                txtDienThoai.Clear();
                txtDiaChi.Clear();
                txtBienSo.Clear();

                txtTenChuXe.Focus();
                // Refresh list tiếp nhận gần đây và widgets
                try
                {
                    var recent = await App.DB.GetRecentReceiptsAsync(5);

                    var stats = await App.DB.GetDashboardStatsAsync();
                    txtLuotXeHienTai.Text = stats.LuotXeTrongNgay.ToString();
                    txtLuotXeToiDa.Text = $"/ {stats.MaxDailyVehicles} xe";
                    double phanTram = (double)stats.LuotXeTrongNgay / stats.MaxDailyVehicles * 100;
                    txtPhanTramCongSuat.Text = $"{Math.Round(phanTram)}% công suất";
                    double progressWidth = 310;
                    rectProgressBar.Width = (phanTram / 100.0) * progressWidth;
                    int xeConLai = stats.MaxDailyVehicles - stats.LuotXeTrongNgay;
                    if (xeConLai < 0) xeConLai = 0;
                    txtTrangThaiVanHanh.Text = phanTram >= 80 ? "Gara đang ở mức vận hành cao. Bạn có thể tiếp nhận thêm tối đa" :
                                              (phanTram >= 50 ? "Gara đang ở mức vận hành trung bình. Bạn có thể tiếp nhận thêm tối đa" :
                                                                "Gara đang ở mức vận hành thấp. Bạn có thể tiếp nhận thêm tối đa");
                    txtXeConLai.Text = $" {xeConLai} xe ";
                }
                catch { }
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}