using System.Windows;

namespace WpfApp1
{
    public partial class QuyDinhHistoryPopup : Window
    {
        public QuyDinhHistoryPopup()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
