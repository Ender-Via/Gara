using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnTiepNhan_Click(object sender, RoutedEventArgs e)
        {
            TiepNhanWindow window = new TiepNhanWindow();
            window.ShowDialog();
        }

        private void BtnSuaChua_Click(object sender, RoutedEventArgs e)
        {
            PhieuSuaChuaWindow window = new PhieuSuaChuaWindow();
            window.ShowDialog();
        }

        private void BtnTraCuu_Click(object sender, RoutedEventArgs e)
        {
            TraCuuWindow window = new TraCuuWindow();
            window.ShowDialog();
        }

        private void BtnPhieuThu_Click(object sender, RoutedEventArgs e)
        {
            PhieuThuWindow window = new PhieuThuWindow();
            window.ShowDialog();
        }

        private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
        {
            BaoCaoWindow window = new BaoCaoWindow();
            window.ShowDialog();
        }

        private void BtnQuyDinh_Click(object sender, RoutedEventArgs e)
        {
            QuyDinhWindow window = new QuyDinhWindow();
            window.ShowDialog();
        }
    }
}