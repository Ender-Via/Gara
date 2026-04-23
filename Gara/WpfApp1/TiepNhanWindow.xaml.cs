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
    /// Interaction logic for TiepNhanWindow.xaml
    /// </summary>
    public partial class TiepNhanWindow : Window
    {
        public TiepNhanWindow()
        {
            InitializeComponent();
            dpNgayTiepNhan.SelectedDate = System.DateTime.Now;
            LoadHieuXe();
        }

        private void LoadHieuXe()
        {
            // Dummy data based on the image description
            // Replace this with actual database loading logic later
            cmbHieuXe.Items.Add("Toyota");
            cmbHieuXe.Items.Add("Honda");
            cmbHieuXe.Items.Add("Suzuki");
            cmbHieuXe.Items.Add("Ford");
            cmbHieuXe.Items.Add("Kia");
            cmbHieuXe.Items.Add("Hyundai");
            cmbHieuXe.Items.Add("Mazda");
            cmbHieuXe.Items.Add("BMW");
            cmbHieuXe.Items.Add("Mercedes");
            cmbHieuXe.Items.Add("Nissan");
            if (cmbHieuXe.Items.Count > 0)
                cmbHieuXe.SelectedIndex = 0;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đã lưu thông tin tiếp nhận!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
