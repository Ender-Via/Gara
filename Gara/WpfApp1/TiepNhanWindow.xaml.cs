using System;
using System.Linq;
using System.Windows;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách hiệu xe: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnLuu_Click(object sender, RoutedEventArgs e)
        {

            string tenKhach = txtTenChuXe.Text.Trim();
            string sdt = txtDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string bienSo = txtBienSo.Text.Trim();

            string tenHieuXe = cmbHieuXe.Text;

            DateTime ngayTiepNhan = dpNgayTiepNhan.SelectedDate ?? DateTime.Now;


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
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}