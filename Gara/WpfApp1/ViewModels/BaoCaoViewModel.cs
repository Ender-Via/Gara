using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;

namespace WpfApp1.ViewModels
{
    public partial class BaoCaoViewModel : ObservableObject
    {
        private List<BaoCao> AllData;

        [ObservableProperty]
        private ChartValues<double> _doanhThuNgay;

        [ObservableProperty]
        private ChartValues<int> _soLuotSua;

        [ObservableProperty]
        private string[] _days;

        [ObservableProperty]
        private double _doanhThu;

        [ObservableProperty]
        private int _luotSua;

        [ObservableProperty]
        private int _khachHang;

        [ObservableProperty]
        private List<BaoCao> _baoCaoChiTiet;

        [ObservableProperty]
        private string _thangDuocChon;

        [ObservableProperty]
        private string _namDuocChon;

        public BaoCaoViewModel()
        {
            AllData = new List<BaoCao>()
            {
                new BaoCao { Date = DateTime.Now, TenKH = "A", DVu = "Thay dầu", TongTien = 300000 },
                new BaoCao { Date = DateTime.Now.AddDays(-1), TenKH = "B", DVu = "Rửa xe", TongTien = 100000 },
                new BaoCao { Date = DateTime.Now.AddDays(-1), TenKH = "C", DVu = "Sửa máy", TongTien = 500000 }
            };
        }

        [RelayCommand]
        private void XemBaoCao()
        {
            if (string.IsNullOrEmpty(ThangDuocChon) || string.IsNullOrEmpty(NamDuocChon))
            {
                MessageBox.Show("Chọn tháng và năm!");
                return;
            }

            int month = int.Parse(ThangDuocChon);
            int year = int.Parse(NamDuocChon);

            LoadBaoCao(month, year);
        }

        private void LoadBaoCao(int month, int year)
        {
            var filtered = AllData
                .Where(x => x.Date.Month == month && x.Date.Year == year)
                .ToList();

            BaoCaoChiTiet = filtered;

            var grouped = filtered
                .GroupBy(x => x.Date.Day)
                .OrderBy(g => g.Key)
                .ToList();

            DoanhThuNgay = new ChartValues<double>(
                grouped.Select(g => g.Sum(x => x.TongTien))
            );

            SoLuotSua = new ChartValues<int>(
                grouped.Select(g => g.Count())
            );

            Days = grouped.Select(g => g.Key.ToString()).ToArray();

            DoanhThu = filtered.Sum(x => x.TongTien);
            LuotSua = filtered.Count();
            KhachHang = filtered.Select(x => x.TenKH).Distinct().Count();
        }
    }

    public class BaoCao
    {
        public DateTime Date { get; set; }
        public string TenKH { get; set; }
        public string DVu { get; set; }
        public double TongTien { get; set; }
    }
}
