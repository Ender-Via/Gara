using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public partial class QuyDinhWindow : Window
    {
        private readonly Dictionary<string, int> _defaultValues = new()
        {
            { "QD1_MAX_BRANDS",      10  },
            { "QD1_MAX_CARS_PER_DAY", 30 },
            { "QD2_MAX_PART_TYPES",  100 },
            { "QD2_MAX_LABOR_TYPES",  200 }
        };

        private SystemRegulation _currentReg;
        private QuyDinhDashboardStats _dashboardStats;
        private readonly QuyDinhViewModel _viewModel;

        public QuyDinhWindow()
        {
            InitializeComponent();
            _viewModel = new QuyDinhViewModel();
            Loaded += QuyDinhWindow_Loaded;
        }

        private async void QuyDinhWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu quy định: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadDataAsync()
        {
            _currentReg = await _viewModel.GetSystemRegulationsAsync();

            txtQd1HieuXe.Text = (_currentReg?.MaxCarBrands ?? _defaultValues["QD1_MAX_BRANDS"]).ToString();
            txtQd1SoXe.Text = (_currentReg?.MaxDailyVehicles ?? _defaultValues["QD1_MAX_CARS_PER_DAY"]).ToString();
            txtQd2VatTu.Text = (_currentReg?.MaxParts ?? _defaultValues["QD2_MAX_PART_TYPES"]).ToString();
            txtQd2TienCong.Text = (_currentReg?.MaxLabors ?? _defaultValues["QD2_MAX_LABOR_TYPES"]).ToString();

            await LoadDashboardStatsAsync();
        }

        private async Task LoadDashboardStatsAsync()
        {
            _dashboardStats = await _viewModel.GetQuyDinhDashboardStatsAsync();

            var hieuXeHienTai = _dashboardStats?.HieuXeHienTai ?? 0;
            var hieuXeToiDa = Math.Max(1, _dashboardStats?.HieuXeToiDa ?? 0);
            var luotTrungBinh = _dashboardStats?.LuotXeTrungBinhNgay ?? 0m;
            var luotToiDa = Math.Max(1, _dashboardStats?.LuotXeToiDaNgay ?? 0);
            var dichVuDangNiemYet = _dashboardStats?.DichVuDangNiemYet ?? 0;
            var dichVuToiDa = Math.Max(1, _dashboardStats?.DichVuToiDa ?? 0);

            txtHieuXeHienTai.Text = $"{hieuXeHienTai} / {hieuXeToiDa}";
            pbHieuXe.Maximum = hieuXeToiDa;
            pbHieuXe.Value = Math.Min(hieuXeHienTai, hieuXeToiDa);

            txtLuotXeTrungBinh.Text = $"{luotTrungBinh:0.#} / {luotToiDa}";
            pbLuotXe.Maximum = luotToiDa;
            pbLuotXe.Value = Math.Min((double)luotTrungBinh, luotToiDa);

            txtDichVuDangNiemYet.Text = $"{dichVuDangNiemYet} / {dichVuToiDa}";
            pbDichVu.Maximum = dichVuToiDa;
            pbDichVu.Value = Math.Min(dichVuDangNiemYet, dichVuToiDa);
        }

        private async void BtnResetDefault_Click(object sender, RoutedEventArgs e)
        {
            txtQd1HieuXe.Text = _defaultValues["QD1_MAX_BRANDS"].ToString();
            txtQd1SoXe.Text = _defaultValues["QD1_MAX_CARS_PER_DAY"].ToString();
            txtQd2VatTu.Text = _defaultValues["QD2_MAX_PART_TYPES"].ToString();
            txtQd2TienCong.Text = _defaultValues["QD2_MAX_LABOR_TYPES"].ToString();

            await UpdateRegulationsAsync();
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            await UpdateRegulationsAsync();
        }

        private async Task UpdateRegulationsAsync()
        {
            if (!TryGetPositiveInt(txtQd1HieuXe.Text, out var qd1Brands) ||
                !TryGetPositiveInt(txtQd1SoXe.Text, out var qd1Cars) ||
                !TryGetPositiveInt(txtQd2VatTu.Text, out var qd2Parts) ||
                !TryGetPositiveInt(txtQd2TienCong.Text, out var qd2Labors))
            {
                MessageBox.Show("Vui lòng nhập toàn bộ giá trị là số nguyên dương.",
                                "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _viewModel.UpsertSystemRegulationAsync(
                    qd1Brands, qd1Cars, qd2Parts, qd2Labors, _currentReg);

                await LoadDataAsync();
                MessageBox.Show("Cập nhật quy định thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool TryGetPositiveInt(string value, out int result)
        {
            return int.TryParse(value, out result) && result > 0;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        private class HistoryRow
        {
            public string ThoiGian { get; set; }
            public string QuyDinh { get; set; }
            public string GiaTriCu { get; set; }
            public string GiaTriMoi { get; set; }
            public string NguoiThucHien { get; set; }
        }
    }
}