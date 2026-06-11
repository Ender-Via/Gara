using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;

namespace WpfApp1.ViewModels
{
    public class PhieuSuaChuaViewModel
    {
        public async Task<List<string>> GetLicensePlatesAsync()
        {
            var response = await App.DB._client.From<Vehicle>().Get();
            return response.Models?.Select(x => x.LicensePlate).ToList() ?? new List<string>();
        }

        public async Task<List<Part>> GetPartsAsync()
        {
            var response = await App.DB._client.From<Part>().Get();
            return response.Models ?? new List<Part>();
        }

        public async Task<List<Labor>> GetLaborsAsync()
        {
            var response = await App.DB._client.From<Labor>().Get();
            return response.Models ?? new List<Labor>();
        }

        public async Task<bool> LuuPhieuSuaChuaAsync(string licensePlate, DateTime ngaySuaChua, IEnumerable<RepairOrderDetail> danhSachChiTiet)
        {
            return await App.DB.LuuPhieuSuaChuaAsync(licensePlate, ngaySuaChua, danhSachChiTiet);
        }

        public decimal CalculateTotal(IEnumerable<RepairOrderDetail> details)
        {
            return details.Sum(x => x.LineTotal ?? 0);
        }
    }
}
