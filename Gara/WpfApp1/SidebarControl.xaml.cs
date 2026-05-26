using System.Windows;
using System.Windows.Controls;

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
            Loaded += (_, _) => UpdateActiveItem();
        }

        public string ActiveItem
        {
            get => (string)GetValue(ActiveItemProperty);
            set => SetValue(ActiveItemProperty, value);
        }

        private static void OnActiveItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SidebarControl sidebar)
            {
                sidebar.UpdateActiveItem();
            }
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
