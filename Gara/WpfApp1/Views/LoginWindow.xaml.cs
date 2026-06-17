using System.Windows;
using WpfApp1.Models.Entities;

namespace WpfApp1.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Vui lòng nhập đầy đủ email và mật khẩu.");
                return;
            }

            btnLogin.IsEnabled = false;
            lblError.Visibility = Visibility.Collapsed;

            try
            {
                var user = await App.DB.LoginAsync(email, password);
                if (user != null)
                {
                    App.CurrentUser = user;

                    // Mở màn hình chính (mặc định là Tiếp Nhận)
                    var mainWindow = new TiepNhanWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Email hoặc mật khẩu không chính xác.");
                }
            }
            catch (System.Exception ex)
            {
                ShowError("Lỗi kết nối: " + ex.Message);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
