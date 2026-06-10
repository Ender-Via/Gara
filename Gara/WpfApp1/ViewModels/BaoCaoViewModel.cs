using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp1.Models.DTOs;

namespace WpfApp1.ViewModels
{
    public class BaoCaoViewModel
    {
        public async Task<List<BaoCaoDoanhThuRow>> GetBaoCaoDoanhSoAsync(int month, int year)
        {
            return await App.DB.GetBaoCaoDoanhSoAsync(month, year);
        }

        public async Task<List<BaoCaoTonKhoRow>> GetBaoCaoTonKhoAsync(int month, int year)
        {
            return await App.DB.GetBaoCaoTonKhoAsync(month, year);
        }
    }
}
