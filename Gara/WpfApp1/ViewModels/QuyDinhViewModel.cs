using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;

namespace WpfApp1.ViewModels
{
    public class QuyDinhViewModel
    {
        public async Task<SystemRegulation> GetSystemRegulationsAsync()
        {
            return await App.DB.GetSystemRegulationsAsync();
        }

        public async Task<QuyDinhDashboardStats> GetQuyDinhDashboardStatsAsync()
        {
            return await App.DB.GetQuyDinhDashboardStatsAsync();
        }

        public async Task UpsertSystemRegulationAsync(int maxBrands, int maxVehicles, int maxParts, int maxLabors, SystemRegulation oldReg)
        {
            await App.DB.UpsertSystemRegulationAsync(maxBrands, maxVehicles, maxParts, maxLabors, oldReg);
        }
    }
}
