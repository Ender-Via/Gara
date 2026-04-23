using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp1.ViewModels
{
    public partial class QuyDinhViewModel : ObservableObject
    {
        [RelayCommand]
        private void Save()
        {
            MessageBox.Show("Lưu thành công!");
        }

        [RelayCommand]
        private void Dong(Window window)
        {
            window?.Close();
        }
    }
}