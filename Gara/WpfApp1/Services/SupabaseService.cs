using Supabase;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using System.Linq;

namespace WpfApp1.Services
{
    /// <summary>
    /// [THIN SERVICE]: Lớp này hiện chỉ đóng vai trò Data Access Provider thuần túy.
    /// Không chứa logic nghiệp vụ, không chứa logic điều phối quy trình.
    /// </summary>
    public class SupabaseService
    {
        public Supabase.Client _client;

        public async Task InitializeAsync()
        {
            var url = "https://klsafpqatqpohiqykoud.supabase.co";
            var key = "sb_publishable_3F5w0GD2L9c_sNGD3wfpvQ_ZCGOLM-1";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(url, key, options);
            await _client.InitializeAsync();
        }

        public async Task<NhanVien?> LoginAsync(string email, string password)
        {
            try
            {
                // Sử dụng Supabase Auth để đăng nhập
                var session = await _client.Auth.SignIn(email, password);
                if (session != null)
                {
                    // Sau khi đăng nhập thành công, lấy thông tin nhân viên từ bảng nhan_vien
                    var result = await _client.From<NhanVien>()
                        .Where(x => x.Email == email)
                        .Get();

                    var user = result.Models.FirstOrDefault();
                    if (user == null)
                    {
                        System.Windows.MessageBox.Show($"Đăng nhập thành công nhưng KHÔNG tìm thấy email '{email}' trong bảng nhân viên. Hãy kiểm tra lại bảng dữ liệu.");
                        return null;
                    }
                    return user;
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi: " + ex.Message);
            }
            return null;
        }
    }
}
