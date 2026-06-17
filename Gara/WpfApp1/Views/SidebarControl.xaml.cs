using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models.Entities;

namespace WpfApp1
{
    public partial class SidebarControl : UserControl
    {
        public static readonly DependencyProperty ActiveItemProperty =
            DependencyProperty.Register(
                nameof(ActiveItem),
                typeof(string),
                typeof(SidebarControl),
                new PropertyMetadata(string.Empty, OnActiveItemChanged));

        public SidebarControl()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                UpdateActiveItem();
                ApplyPermissions();
            };
        }

        private void ApplyPermissions()
        {
            var user = App.CurrentUser;
            if (user == null) return;

            // Hiển thị tên và vai trò
            txtUserName.Text = user.HoTen;
            txtUserRole.Text = GetRoleDisplayName(user.Role);

            // Mặc định ẩn tất cả (hoặc hiện tùy thiết kế, ở đây ta sẽ ẩn theo role)
            // Admin: Hiện tất cả
            if (user.Role == UserRole.Admin) return;

            // Lễ Tân: BM1, BM3
            Bm1Button.Visibility = (user.Role == UserRole.LeTan) ? Visibility.Visible : Visibility.Collapsed;
            Bm3Button.Visibility = (user.Role == UserRole.LeTan || user.Role == UserRole.KyThuatVien || user.Role == UserRole.KeToan) ? Visibility.Visible : Visibility.Collapsed;

            // Kỹ Thuật Viên: BM2, BM3
            Bm2Button.Visibility = (user.Role == UserRole.KyThuatVien) ? Visibility.Visible : Visibility.Collapsed;

            // Kế Toán: BM3, BM4, BM5
            Bm4Button.Visibility = (user.Role == UserRole.KeToan) ? Visibility.Visible : Visibility.Collapsed;
            Bm5Button.Visibility = (user.Role == UserRole.KeToan) ? Visibility.Visible : Visibility.Collapsed;

            // Quy Định: Chỉ Admin (đã return ở trên nếu là admin)
            Qd6Button.Visibility = Visibility.Collapsed;
        }

        private string GetRoleDisplayName(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Quản trị viên",
                UserRole.LeTan => "Lễ tân",
                UserRole.KyThuatVien => "Kỹ thuật viên",
                UserRole.KeToan => "Kế toán",
                _ => "Nhân viên"
            };
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                var loginWindow = new Views.LoginWindow();
                loginWindow.Show();
                Window.GetWindow(this)?.Close();
            }
        }

        public string ActiveItem
        {
            get => (string)GetValue(ActiveItemProperty);
            set => SetValue(ActiveItemProperty, value);
        }

        private static void OnActiveItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarControl sidebar)
                sidebar.UpdateActiveItem();
        }

        private void UpdateActiveItem()
        {
            Bm1Button.IsChecked = ActiveItem == "BM1";
            Bm2Button.IsChecked = ActiveItem == "BM2";
            Bm3Button.IsChecked = ActiveItem == "BM3";
            Bm4Button.IsChecked = ActiveItem == "BM4";
            Bm5Button.IsChecked = ActiveItem == "BM5";
            Qd6Button.IsChecked = ActiveItem == "QD6";
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton button || button.CommandParameter is not string targetKey)
                return;

            if (targetKey == ActiveItem)
            {
                UpdateActiveItem();
                return;
            }

            var currentWindow = Window.GetWindow(this);
            var targetWindow = CreateWindow(targetKey);
            if (targetWindow == null)
            {
                UpdateActiveItem();
                return;
            }

            targetWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            targetWindow.Show();
            currentWindow?.Close();
        }

        private static Window? CreateWindow(string targetKey)
        {
            return targetKey switch
            {
                "BM1" => new TiepNhanWindow(),
                "BM2" => new PhieuSuaChuaWindow(),
                "BM3" => new TraCuuWindow(),
                "BM4" => new PhieuThuWindow(),
                "BM5" => new BaoCaoWindow(),
                "QD6" => new QuyDinhWindow(),
                _ => null
            };
        }

    }
}