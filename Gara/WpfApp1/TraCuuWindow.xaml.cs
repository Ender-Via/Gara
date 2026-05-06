using System;
using System.Collections.Generic;
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
    /// Interaction logic for TraCuuWindow.xaml
    /// </summary>
    public partial class TraCuuWindow : Window
    {
        public TraCuuWindow()
        {
            InitializeComponent();
            Loaded += TraCuuWindow_Loaded;
            btnTimKiem.Click += btnTimKiem_Click;
        }
        private async void TraCuuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void btnTimKiem_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }
        private async Task LoadDataAsync()
        {
            try
            {
                string bienSo = txtBienSoSearch.Text.Trim();
                List<TraCuuXeRow> data = await App.DB.TraCuuXeAsync(bienSo);
                dgDanhSachXe.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
