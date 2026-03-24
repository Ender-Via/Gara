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
using System.IO;
using System.Text.Json;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for PhieuThuWindow.xaml
    /// </summary>
    public partial class PhieuThuWindow : Window
    {
        //test json
        private string filePath = "phieuthu.json";

        private List<PhieuThu> LoadFromFile()
        {
            if (!File.Exists(filePath))
                return new List<PhieuThu>();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<PhieuThu>>(json) ?? new List<PhieuThu>();
        }

        private void SaveToFile(List<PhieuThu> data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public class PhieuThu
        {
            public string MaPhieu { get; set; }

            public string TenKH { get; set; }
            public string Email { get; set; }

            public string SoDienThoai { get; set; }

            public string BienSoXe { get; set; }

            public string HieuXe { get; set; }

            public DateTime NgayThu { get; set; }

            public double SoTien { get; set; }

            public string GhiChu { get; set; }
        }

        List<PhieuThu> AllPhieuThu = new List<PhieuThu>();

        public PhieuThuWindow()
        {
            InitializeComponent();
            AllPhieuThu = LoadFromFile();
            GenerateMaPhieu();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void GenerateMaPhieu()
        {
            int max = AllPhieuThu
                .Select(x => int.Parse(x.MaPhieu.Substring(2)))
                .DefaultIfEmpty(0)
                .Max();

            txtMaPhieu.Text = "PT" + (max + 1).ToString("D3");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            double soTien;
            if (!double.TryParse(txtSoTien.Text, out soTien))
            {
                MessageBox.Show("Số tiền không hợp lệ!");
                return;
            }

            // lấy ghi chú (TextBox version)
            string ghiChu = new TextRange( txtGhiChu.Document.ContentStart, txtGhiChu.Document.ContentEnd ).Text;

            PhieuThu pt = new PhieuThu()
            {
                MaPhieu = txtMaPhieu.Text,
                TenKH = txtTenKH.Text,
                Email = txtEmail.Text,
                SoDienThoai = txtSDT.Text,
                BienSoXe = txtBienSo.Text,
                HieuXe = txtHieuXe.Text,
                NgayThu = dpNgayThu.SelectedDate ?? DateTime.Now,
                SoTien = double.Parse(txtSoTien.Text),
                GhiChu = ghiChu
            };
            AllPhieuThu.Add(pt);
            SaveToFile(AllPhieuThu);

            MessageBox.Show("Thu tiền thành công!");

            // tạo mã mới
            GenerateMaPhieu();

            // reset form
            txtTenKH.Clear();
            txtEmail.Clear();
            txtSDT.Clear();
            txtBienSo.Clear();
            txtHieuXe.Clear();
            txtSoTien.Clear();
            txtGhiChu.Document.Blocks.Clear();
            dpNgayThu.SelectedDate = DateTime.Now;
        }

        private void txtMaPhieu_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
