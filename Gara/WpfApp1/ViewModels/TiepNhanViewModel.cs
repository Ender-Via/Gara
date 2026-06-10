using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;
using WpfApp1.Services;

namespace WpfApp1.ViewModels
{
    public class TiepNhanViewModel
    {
        private readonly SupabaseService _db;

        public TiepNhanViewModel()
        {
            _db = App.DB;
        }

        public async Task<List<CarBrand>> GetCarBrandsAsync()
        {
            var response = await _db._client.From<CarBrand>().Get();
            return response.Models ?? new List<CarBrand>();
        }

        public async Task<TraCuuDashBoardStats> GetDashboardStatsAsync()
        {
            return await _db.GetDashboardStatsAsync();
        }

        public async Task<List<RecentReceiptDTO>> GetRecentReceiptsAsync(int limit = 5)
        {
            return await _db.GetRecentReceiptsAsync(limit);
        }

        public async Task<bool> LuuTiepNhanXeAsync(string tenKhach, string sdt, string diaChi, string bienSo, string tenHieuXe, DateTime ngayTiepNhan)
        {
            return await _db.LuuTiepNhanXeAsync(tenKhach, sdt, diaChi, bienSo, tenHieuXe, ngayTiepNhan);
        }
    }
}
