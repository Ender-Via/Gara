using WpfApp1.Views;

namespace WpfApp1.Services
{
    public class WindowNavigationService : IWindowNavigationService
    {
        public void OpenTiepNhan()
        {
            var window = new TiepNhanWindow();
            window.ShowDialog();
        }

        public void OpenSuaChua()
        {
            var window = new PhieuSuaChuaWindow();
            window.ShowDialog();
        }

        public void OpenTraCuu()
        {
            var window = new TraCuuWindow();
            window.ShowDialog();
        }

        public void OpenPhieuThu()
        {
            var window = new PhieuThuWindow();
            window.ShowDialog();
        }

        public void OpenBaoCao()
        {
            var window = new BaoCaoWindow();
            window.ShowDialog();
        }

        public void OpenQuyDinh()
        {
            var window = new QuyDinhWindow();
            window.ShowDialog();
        }
    }
}
