using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class BaoCaoWindow : Window
    {
        public List<BaoCaoDoanhThuRow> BaoCaoDoanhThu { get; set; }
        public List<BaoCaoTonKhoRow> BaoCaoTonVatTu { get; set; }

        public BaoCaoWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void BtnXemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang.SelectedItem == null || cbNam.SelectedItem == null)
            {
                MessageBox.Show("Chọn tháng và năm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int month = int.Parse(((ComboBoxItem)cbThang.SelectedItem).Content.ToString());
            int year = int.Parse(((ComboBoxItem)cbNam.SelectedItem).Content.ToString());

            BtnXemBaoCao.IsEnabled = false;

            try
            {
                BaoCaoDoanhThu = await App.DB.GetBaoCaoDoanhSoAsync(month, year);
                BaoCaoTonVatTu = await App.DB.GetBaoCaoTonKhoAsync(month, year);

                dgvDoanhSo.ItemsSource = BaoCaoDoanhThu;
                dgvTonKho.ItemsSource = BaoCaoTonVatTu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnXemBaoCao.IsEnabled = true;
            }
        }
    }
}