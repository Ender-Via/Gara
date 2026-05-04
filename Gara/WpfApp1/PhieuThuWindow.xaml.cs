using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
// Dùng thư viện của PostgreSQL

namespace WpfApp1
{
    public partial class PhieuThuWindow : Window
    {
        // 1. Dán chuỗi kết nối ADO.NET của Supabase vào đây
        private string connectionString = "Server=YOUR_SUPABASE_HOST;Port=6543;Database=postgres;User Id=YOUR_USER;Password=YOUR_PASSWORD;";

        public PhieuThuWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtSoTien.Text, out double soTien))
            {
                MessageBox.Show("Số tiền không hợp lệ!");
                return;
            }
            string bienSo = txtBienSo.Text.Trim();
            if (string.IsNullOrEmpty(bienSo))
            {
                MessageBox.Show("Vui lòng nhập Biển số xe!");
                return;
            }
        }
        private void txtMaPhieu_TextChanged(object sender, RoutedEventArgs e)
        {
            
        }
    }
}