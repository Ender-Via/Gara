using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class QuyDinhWindow : Window
    {
        private readonly Dictionary<string, string> _defaultValues = new()
        {
            { "QD1_MAX_BRANDS", "10" },
            { "QD1_MAX_CARS_PER_DAY", "30" },
            { "QD2_MAX_PART_TYPES", "100" },
            { "QD2_MAX_LABOR_TYPES", "20" }
        };

        private readonly Dictionary<string, string> _currentValues = new();
        private readonly ObservableCollection<HistoryRow> _history = new();

        public QuyDinhWindow()
        {
            InitializeComponent();
            Loaded += QuyDinhWindow_Loaded;
            WireButtons();
        }

        private async void QuyDinhWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu quy định: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WireButtons()
        {
            foreach (var button in FindVisualChildren<Button>(this))
            {
                var text = button.Content?.ToString()?.Trim();
                if (text == "Đặt lại mặc định") button.Click += BtnResetDefault_Click;
                if (text == "Cập nhật quy định") button.Click += BtnUpdate_Click;
            }
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            if (App.DB == null)
            {
                throw new InvalidOperationException("Chưa khởi tạo kết nối Supabase.");
            }

            var regs = await App.DB.GetSystemRegulationsAsync();
            _currentValues.Clear();
            foreach (var reg in regs)
            {
                if (!string.IsNullOrWhiteSpace(reg.RegulationKey))
                    _currentValues[reg.RegulationKey] = reg.RegulationValue ?? string.Empty;
            }

            txtQd1HieuXe.Text = GetRegValue("QD1_MAX_BRANDS");
            txtQd1SoXe.Text = GetRegValue("QD1_MAX_CARS_PER_DAY");
            txtQd2VatTu.Text = GetRegValue("QD2_MAX_PART_TYPES");
            txtQd2TienCong.Text = GetRegValue("QD2_MAX_LABOR_TYPES");

            await ReloadHistoryAsync();
        }

        private async System.Threading.Tasks.Task ReloadHistoryAsync()
        {
            var history = await App.DB.GetSystemRegulationHistoryAsync();
            _history.Clear();
            foreach (var item in history.OrderByDescending(x => x.ChangedAt))
            {
                _history.Add(new HistoryRow
                {
                    ThoiGian = item.ChangedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    QuyDinh = item.RegulationKey,
                    GiaTriCu = string.IsNullOrWhiteSpace(item.OldValue) ? "-" : item.OldValue,
                    GiaTriMoi = item.NewValue,
                    NguoiThucHien = string.IsNullOrWhiteSpace(item.ChangedBy) ? "Quản trị viên" : item.ChangedBy
                });
            }

            dgvHistory.ItemsSource = _history;
        }

        private async void BtnViewAllHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ReloadHistoryAsync();

                var popup = new QuyDinhHistoryPopup
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                popup.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải lịch sử quy định: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetRegValue(string key)
        {
            return _currentValues.TryGetValue(key, out var value) ? value : _defaultValues[key];
        }

        private async void BtnResetDefault_Click(object sender, RoutedEventArgs e)
        {
            txtQd1HieuXe.Text = _defaultValues["QD1_MAX_BRANDS"];
            txtQd1SoXe.Text = _defaultValues["QD1_MAX_CARS_PER_DAY"];
            txtQd2VatTu.Text = _defaultValues["QD2_MAX_PART_TYPES"];
            txtQd2TienCong.Text = _defaultValues["QD2_MAX_LABOR_TYPES"];
            await ReloadHistoryAsync();
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetPositiveInt(txtQd1HieuXe.Text, out var qd1Brands) ||
                !TryGetPositiveInt(txtQd1SoXe.Text, out var qd1Cars) ||
                !TryGetPositiveInt(txtQd2VatTu.Text, out var qd2Parts) ||
                !TryGetPositiveInt(txtQd2TienCong.Text, out var qd2Labors))
            {
                MessageBox.Show("Vui lòng nhập toàn bộ giá trị là số nguyên dương.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await App.DB.UpsertSystemRegulationAsync("QD1_MAX_BRANDS", qd1Brands.ToString(), "Số lượng hiệu xe tối đa");
            await App.DB.UpsertSystemRegulationAsync("QD1_MAX_CARS_PER_DAY", qd1Cars.ToString(), "Số xe sửa chữa tối đa trong ngày");
            await App.DB.UpsertSystemRegulationAsync("QD2_MAX_PART_TYPES", qd2Parts.ToString(), "Số loại vật tư phụ tùng tối đa");
            await App.DB.UpsertSystemRegulationAsync("QD2_MAX_LABOR_TYPES", qd2Labors.ToString(), "Số loại tiền công tối đa");

            await LoadDataAsync();
            MessageBox.Show("Cập nhật quy định thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (child is T t)
                    yield return t;

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
