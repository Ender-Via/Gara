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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for QuyDinhWindow.xaml
    /// </summary>
    public partial class QuyDinhWindow : Window
    {
        public QuyDinhWindow()
        {
            InitializeComponent();
            Loaded += QuyDinhWindow_Loaded;
        }

        private async void QuyDinhWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var regulations = await App.DB.GetRegulationsAsync();
                txtHieuXe.Text = (regulations?.MaxCarBrands ?? 10).ToString();
                txtSoXeToiDa.Text = (regulations?.MaxDailyVehicles ?? 30).ToString();
                txtVatTu.Text = (regulations?.MaxParts ?? 200).ToString();
                txtTienCong.Text = (regulations?.MaxLabors ?? 100).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải quy định: " + ex.Message);
            }
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtHieuXe.Text, out var maxBrands) || maxBrands <= 0)
            {
                MessageBox.Show("Số lượng hiệu xe không hợp lệ.");
                return;
            }

            if (!int.TryParse(txtSoXeToiDa.Text, out var maxDailyVehicles) || maxDailyVehicles <= 0)
            {
                MessageBox.Show("Số xe tối đa/ngày không hợp lệ.");
                return;
            }

            if (!int.TryParse(txtVatTu.Text, out var maxParts) || maxParts <= 0)
            {
                MessageBox.Show("Số loại vật tư không hợp lệ.");
                return;
            }

            if (!int.TryParse(txtTienCong.Text, out var maxLabors) || maxLabors <= 0)
            {
                MessageBox.Show("Số loại tiền công không hợp lệ.");
                return;
            }

            try
            {
                await App.DB.SaveRegulationsAsync(maxBrands, maxDailyVehicles, maxParts, maxLabors);
                MessageBox.Show("Lưu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu quy định: " + ex.Message);
            }
        }
    }
}
