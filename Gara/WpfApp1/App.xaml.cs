using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp1.Services;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SupabaseService DB { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            DB = new SupabaseService();


            await DB.InitializeAsync();


            MessageBox.Show("Đã kết nối Supabase thành công!");
        }
    }

}
