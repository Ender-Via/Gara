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
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for PhieuThuWindow.xaml
    /// </summary>
    public partial class PhieuThuWindow : Window
    {
        public PhieuThuWindow()
        {
            InitializeComponent();
            DataContext = new PhieuThuViewModel();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtMaPhieu_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
