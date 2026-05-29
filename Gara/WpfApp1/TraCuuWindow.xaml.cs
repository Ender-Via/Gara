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
    public class XeItem
    {
        public string STT { get; set; }
        public string BienSo { get; set; }
        public string HieuXe { get; set; }
        public string ChuXe { get; set; }
        public string TienNo { get; set; }
        public string Color { get; set; }
    }

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
                
                List<TraCuuXeRow> data = await App.DB.TraCuuXeAsync(bienSo);
                
                // Map to UI model format
                var uiData = data.Select((item, index) => new XeItem
                {
                    STT = (index + 1).ToString(),
                    BienSo = item.BienSo,
                    HieuXe = item.HieuXe,
                    ChuXe = item.ChuXe,
                    TienNo = item.TienNo.ToString("N0") + " đ",
                    Color = item.TienNo > 0 ? "#D93025" : "#222222"
                }).ToList();

                dgDanhSachXe.ItemsSource = uiData;

                txtListSummary.Text = $"Hiển thị 1 - {uiData.Count} của {uiData.Count} xe";

                // Load Stats
                var stats = await App.DB.GetDashboardStatsAsync();
                txtTongSoXe.Text = stats.TongSoXe.ToString("N0");
                txtDangSuaChua.Text = stats.DangSuaChua.ToString("N0");
                txtTongNo.Text = stats.TongNo.ToString("N0") + " đ";
                
                txtHieuSuat.Text = stats.HieuSuatSuaChua.ToString("0") + "%";
                gridHieuSuatBar.Width = 300 * (double)(stats.HieuSuatSuaChua / 100);

                txtLuotXe.Text = $"{stats.LuotXeTrongNgay}/{stats.MaxDailyVehicles}";
                gridLuotXeBar.Width = 220 * Math.Min((double)stats.LuotXeTrongNgay / stats.MaxDailyVehicles, 1.0);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
