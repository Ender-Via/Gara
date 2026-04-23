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
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for BaoCaoWindow.xaml
    /// </summary>
    public partial class BaoCaoWindow : Window
    {
        public BaoCaoWindow()
        {
            InitializeComponent();
            DataContext = new BaoCaoViewModel();
        }
    }
}
