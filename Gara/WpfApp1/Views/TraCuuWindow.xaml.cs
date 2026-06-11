using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models.DTOs;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for TraCuuWindow.xaml
    /// </summary>
    public partial class TraCuuWindow : Window
    {
        private readonly TraCuuViewModel _viewModel;

        public TraCuuWindow()
        {
            InitializeComponent();
            _viewModel = new TraCuuViewModel();
            Loaded += TraCuuWindow_Loaded;
            btnTimKiem.Click += btnTimKiem_Click;
            
            // Handle placeholder logic for search textbox
            txtBienSoSearch.GotFocus += TxtBienSoSearch_GotFocus;
            txtBienSoSearch.LostFocus += TxtBienSoSearch_LostFocus;
        }

        private void TxtBienSoSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBienSoSearch.Text == "Nhập thông tin cần tìm...")
            {
                txtBienSoSearch.Text = "";
                txtBienSoSearch.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void TxtBienSoSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBienSoSearch.Text))
            {
                txtBienSoSearch.Text = "Nhập thông tin cần tìm...";
                txtBienSoSearch.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)); // #888888
            }
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
                if (bienSo == "Nhập thông tin cần tìm...")
                {
                    bienSo = "";
                }

                var data = await _viewModel.TraCuuXeAsync(bienSo);
                var uiData = _viewModel.MapToUiModel(data);

                dgDanhSachXe.ItemsSource = uiData;
                txtListSummary.Text = $"Hiển thị 1 - {uiData.Count} của {uiData.Count} xe";

                // Fetch and display dashboard stats
                var stats = await _viewModel.GetDashboardStatsAsync();
                txtTongSoXe.Text = stats.TongSoXe.ToString("N0");
                txtDangSuaChua.Text = stats.DangSuaChua.ToString("N0");
                txtTongNo.Text = stats.TongNo.ToString("N0") + " đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
