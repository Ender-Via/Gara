using LiveCharts;
using LiveCharts.Wpf;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for BaoCaoWindow.xaml
    /// </summary>
    public partial class BaoCaoWindow : Window
    {
        // giữ dữ liệu gốc
        private List<BaoCao> AllData;

        public ChartValues<double> DoanhThuNgay { get; set; }
        public ChartValues<int> SoLuotSua { get; set; }
        public string[] Days { get; set; }
        public class BaoCao
        {
            public DateTime Date { get; set; }
            public string TenKH { get; set; }
            public string DVu { get; set; }
            public double TongTien { get; set; }
        }
        public double DoanhThu { get; set; }
        public int LuotSua { get; set; }
        public int KhachHang { get; set; }

        public List<BaoCao> BaoCaoChiTiet { get; set; }
        
        
        public BaoCaoWindow()
        {
            InitializeComponent();

            // dữ liệu test
            AllData = new List<BaoCao>()
            {
                new BaoCao { Date = DateTime.Now, TenKH = "A", DVu = "Thay dầu", TongTien = 300000 },
                new BaoCao { Date = DateTime.Now.AddDays(-1), TenKH = "B", DVu = "Rửa xe", TongTien = 100000 },
                new BaoCao { Date = DateTime.Now.AddDays(-1), TenKH = "C", DVu = "Sửa máy", TongTien = 500000 }
            };


            DataContext = this;
        }

        private void LoadBaoCao(int month, int year)
        {

            // 1. Lọc theo tháng
            var filtered = AllData
            .Where(x => x.Date.Month == month && x.Date.Year == year)
            .ToList();

            MessageBox.Show($"Có {filtered.Count} dữ liệu"); // debug
    

            // 2. Gán lại cho DataGrid
            BaoCaoChiTiet = filtered;

            // 3. Group theo ngày
            var grouped = filtered
                .GroupBy(x => x.Date.Day)
                .OrderBy(g => g.Key)
                .ToList();

            // 4. Chart
            DoanhThuNgay = new ChartValues<double>(
                grouped.Select(g => g.Sum(x => x.TongTien))
            );

            SoLuotSua = new ChartValues<int>(
                grouped.Select(g => g.Count())
            );

            Days = grouped.Select(g => g.Key.ToString()).ToArray();

            // 5. Tổng quan
            DoanhThu = filtered.Sum(x => x.TongTien);
            LuotSua = filtered.Count();
            KhachHang = filtered.Select(x => x.TenKH).Distinct().Count();

            // refresh UI
            DataContext = null;
            DataContext = this;
        }

        private void BtnXemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang.SelectedItem == null || cbNam.SelectedItem == null)
            {
                MessageBox.Show("Chọn tháng và năm!");
                return;
            }

            int month = int.Parse(((ComboBoxItem)cbThang.SelectedItem).Content.ToString());
            int year = int.Parse(((ComboBoxItem)cbNam.SelectedItem).Content.ToString());

            LoadBaoCao(month, year);
        }
    }
}
