using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.DTOs;
using WpfApp1.Models.Entities;

namespace WpfApp1.ViewModels
{
    public class BaoCaoViewModel
    {
        public async Task<List<BaoCaoDoanhThuRow>> GetBaoCaoDoanhSoAsync(int month, int year)
        {
            // [DECENTRALIZATION]: ViewModel trực tiếp điều phối luồng lấy dữ liệu thô
            DateTime startDate = new DateTime(year, month, 1).ToUniversalTime();
            DateTime endDate = startDate.AddMonths(1);
            string startStr = startDate.ToString("yyyy-MM-dd");
            string endStr = endDate.ToString("yyyy-MM-dd");

            var repairOrders = (await App.DB._client.From<PhieuSuaChua>()
                .Filter("repair_date", Postgrest.Constants.Operator.GreaterThanOrEqual, startStr)
                .Filter("repair_date", Postgrest.Constants.Operator.LessThan, endStr)
                .Get()).Models ?? new List<PhieuSuaChua>();

            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Get()).Models ?? new List<PhieuTiepNhan>();
            var vehicles = (await App.DB._client.From<Xe>().Get()).Models ?? new List<Xe>();
            var brands = (await App.DB._client.From<HieuXe>().Get()).Models ?? new List<HieuXe>();

            // ViewModel thực hiện join và gom nhóm dữ liệu thô
            var joinedData = from ro in repairOrders
                             join sr in receipts on ro.PhieuTiepNhanId equals sr.Id
                             join v in vehicles on sr.XeId equals v.Id
                             join b in brands on v.HieuXeId equals b.Id
                             select new { BrandName = b.TenHieuXe, TotalAmount = ro.TongTien ?? 0 };

            var groupedData = joinedData
                .GroupBy(x => x.BrandName)
                .Select(g => new BaoCaoDoanhThuRow
                {
                    HieuXe = g.Key,
                    SoLuotSua = g.Count(),
                    ThanhTien = g.Sum(x => x.TotalAmount)
                }).ToList();

            decimal totalRevenue = groupedData.Sum(x => x.ThanhTien);

            // ViewModel gọi hành vi của DTO/Entity để xử lý logic cụ thể
            for (int i = 0; i < groupedData.Count; i++)
            {
                groupedData[i].STT = i + 1;
                groupedData[i].TinhTiLe(totalRevenue);
            }

            return groupedData;
        }

        public async Task<List<BaoCaoTonKhoRow>> GetBaoCaoTonKhoAsync(int month, int year)
        {
            DateTime startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = startDate.AddMonths(1);
            string endStr = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var parts = (await App.DB._client.From<VatTuPhuTung>()
                .Order("part_name", Postgrest.Constants.Ordering.Ascending)
                .Get()).Models ?? new List<VatTuPhuTung>();

            var transactions = (await App.DB._client.From<GiaoDichKho>()
                .Filter("transaction_date", Postgrest.Constants.Operator.LessThan, endStr)
                .Get()).Models ?? new List<GiaoDichKho>();

            // ViewModel yêu cầu Entity VatTuPhuTung tự sinh dòng báo cáo
            return parts.Select((p, index) => p.TaoDongBaoCaoTonKho(transactions, month, year, index + 1)).ToList();
        }
    }
}
