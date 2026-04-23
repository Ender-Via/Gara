using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IWindowNavigationService _navigationService;

        public MainViewModel(IWindowNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void OpenTiepNhan()
        {
            _navigationService.OpenTiepNhan();
        }

        [RelayCommand]
        private void OpenSuaChua()
        {
            _navigationService.OpenSuaChua();
        }

        [RelayCommand]
        private void OpenTraCuu()
        {
            _navigationService.OpenTraCuu();
        }

        [RelayCommand]
        private void OpenPhieuThu()
        {
            _navigationService.OpenPhieuThu();
        }

        [RelayCommand]
        private void OpenBaoCao()
        {
            _navigationService.OpenBaoCao();
        }

        [RelayCommand]
        private void OpenQuyDinh()
        {
            _navigationService.OpenQuyDinh();
        }
    }
}