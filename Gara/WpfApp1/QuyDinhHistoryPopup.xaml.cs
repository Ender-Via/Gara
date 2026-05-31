using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public partial class QuyDinhHistoryPopup : Window
    {
        public QuyDinhHistoryPopup()
        {
            InitializeComponent();
            Loaded += QuyDinhHistoryPopup_Loaded;
        }

        private async void QuyDinhHistoryPopup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var history = await App.DB.GetSystemRegulationHistoryAsync();
                var rows = new ObservableCollection<HistoryRow>();

                foreach (var item in history.OrderByDescending(x => x.ChangedAt))
                {
                    rows.Add(new HistoryRow
                    {
                        ThoiGian = item.ChangedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        QuyDinh = item.RegulationKey,
                        GiaTriCu = string.IsNullOrWhiteSpace(item.OldValue) ? "-" : item.OldValue,
                        GiaTriMoi = item.NewValue,
                        NguoiThucHien = string.IsNullOrWhiteSpace(item.ChangedBy) ? "Qu?n tr? vi�n" : item.ChangedBy
                    });
                }

                dgvAllHistory.ItemsSource = rows;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kh�ng th? t?i l?ch s?: " + ex.Message, "L?i", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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