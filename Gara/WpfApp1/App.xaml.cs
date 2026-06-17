using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp1.Services;
using WpfApp1.Models.Entities;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SupabaseService DB { get; private set; }
        public static NhanVien? CurrentUser { get; set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;


            DB = new SupabaseService();


            await DB.InitializeAsync();


            // MessageBox.Show("Đã kết nối Supabase thành công!");

            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }

}
