using System;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;

namespace WpfApp1.ViewModels
{
    public class QuyDinhViewModel
    {
        public async Task<QuyDinh> GetSystemRegulationsAsync()
        {
            var response = await App.DB._client.From<QuyDinh>().Get();
            return response.Models.FirstOrDefault() ?? new QuyDinh { SoXeTiepNhanToiDa = 30 };
        }

        public async Task<QuyDinhDashboardStats> GetQuyDinhDashboardStatsAsync()
        {
            var stats = new QuyDinhDashboardStats();

            var regulations = await GetSystemRegulationsAsync();
            stats.HieuXeToiDa = regulations?.SoLuongHieuXeToiDa ?? 10;
            stats.LuotXeToiDaNgay = regulations?.SoXeTiepNhanToiDa ?? 30;
            stats.DichVuToiDa = (regulations?.SoLuongVatTuToiDa ?? 100) + (regulations?.SoLuongTienCongToiDa ?? 20);

            var dsXe = (await App.DB._client.From<Xe>().Get()).Models ?? new System.Collections.Generic.List<Xe>();
            stats.HieuXeHienTai = dsXe
                .Select(v => v.HieuXeId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .Count();

            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Get()).Models ?? new System.Collections.Generic.List<PhieuTiepNhan>();
            if (receipts.Any())
            {
                var minDate = receipts.Min(r => r.NgayTiepNhan).Date;
                var maxDate = receipts.Max(r => r.NgayTiepNhan).Date;
                var totalDays = Math.Max(1, (maxDate - minDate).Days + 1);
                stats.LuotXeTrungBinhNgay = Math.Round((decimal)receipts.Count / totalDays, 1);
            }
            else
            {
                stats.LuotXeTrungBinhNgay = 0;
            }

            var orderCount = (await App.DB._client.From<PhieuSuaChua>().Get()).Models?.Count ?? 0;
            stats.DichVuDangNiemYet = orderCount;

            return stats;
        }

        public async Task UpsertSystemRegulationAsync(int maxBrands, int maxVehicles, int maxParts, int maxLabors, QuyDinh oldReg)
        {
            var reg = new QuyDinh
            {
                Id = oldReg?.Id ?? Guid.NewGuid().ToString(),
                SoLuongHieuXeToiDa = maxBrands,
                SoXeTiepNhanToiDa = maxVehicles,
                SoLuongVatTuToiDa = maxParts,
                SoLuongTienCongToiDa = maxLabors
            };

            await App.DB._client.From<QuyDinh>()
                .Upsert(reg, new Postgrest.QueryOptions { OnConflict = "id" });
        }
    }
}
