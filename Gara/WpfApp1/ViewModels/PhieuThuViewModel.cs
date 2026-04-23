using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1;

namespace WpfApp1.ViewModels
{
    public partial class PhieuThuViewModel : ObservableObject
    {
        private string filePath = "phieuthu.json";
        private List<PhieuThu> AllPhieuThu = new List<PhieuThu>();

        [ObservableProperty]
        private string _maPhieu;

        [ObservableProperty]
        private string _tenKH;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _sDT;

        [ObservableProperty]
        private string _bienSo;

        [ObservableProperty]
        private string _hieuXe;

        [ObservableProperty]
        private DateTime _ngayThu = DateTime.Now;

        [ObservableProperty]
        private string _soTien;

        [ObservableProperty]
        private string _ghiChu;


        public PhieuThuViewModel()
        {
            AllPhieuThu = LoadFromFile();
            GenerateMaPhieu();
        }

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

        private void GenerateMaPhieu()
        {
            int max = AllPhieuThu
                .Select(x => int.Parse(x.MaPhieu.Substring(2)))
                .DefaultIfEmpty(0)
                .Max();

            MaPhieu = "PT" + (max + 1).ToString("D3");
        }

        [RelayCommand]
        private void Save()
        {
            if (!double.TryParse(SoTien, out double soTien))
            {
                MessageBox.Show("Số tiền không hợp lệ!");
                return;
            }

            PhieuThu pt = new PhieuThu()
            {
                MaPhieu = MaPhieu,
                TenKH = TenKH,
                Email = Email,
                SoDienThoai = SDT,
                BienSoXe = BienSo,
                HieuXe = HieuXe,
                NgayThu = NgayThu,
                SoTien = soTien,
                GhiChu = GhiChu
            };
            
            AllPhieuThu.Add(pt);
            SaveToFile(AllPhieuThu);

            MessageBox.Show("Thu tiền thành công!");

            GenerateMaPhieu();

            TenKH = string.Empty;
            Email = string.Empty;
            SDT = string.Empty;
            BienSo = string.Empty;
            HieuXe = string.Empty;
            SoTien = string.Empty;
            GhiChu = string.Empty;
            NgayThu = DateTime.Now;
        }

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
}