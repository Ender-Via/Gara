using Supabase;
using System.Threading.Tasks;

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
    }
}
