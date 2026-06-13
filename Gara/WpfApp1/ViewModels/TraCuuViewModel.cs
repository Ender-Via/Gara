using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.DTOs;
using WpfApp1.Models.Entities;
using static Postgrest.Constants;

namespace WpfApp1.ViewModels
{
    public class TraCuuViewModel
    {
        public async Task<List<TraCuuXeRow>> TraCuuXeAsync(string bienSoFilter)
        {
            var xeQuery = App.DB._client.From<Xe>();

            if (!string.IsNullOrWhiteSpace(bienSoFilter))
            {
                var resFiltered = await xeQuery.Filter("license_plate", Operator.ILike, $"%{bienSoFilter}%").Get();
                return await ProcessXeDataAsync(resFiltered.Models);
            }

            var resAll = await xeQuery.Get();
            return await ProcessXeDataAsync(resAll.Models);
        }

        private async Task<List<TraCuuXeRow>> ProcessXeDataAsync(List<Xe> dsXe)
        {
            if (dsXe == null || !dsXe.Any()) return new List<TraCuuXeRow>();

            var customerIds = dsXe.Select(v => v.KhachHangId).Distinct().ToList();
            var brandIds = dsXe.Select(v => v.HieuXeId).Distinct().ToList();

            var customers = (await App.DB._client.From<KhachHang>().Filter("id", Operator.In, customerIds).Get()).Models ?? new List<KhachHang>();
            var brands = (await App.DB._client.From<HieuXe>().Filter("id", Operator.In, brandIds).Get()).Models ?? new List<HieuXe>();
            var customerMap = customers.ToDictionary(x => x.Id);
            var brandMap = brands.ToDictionary(x => x.Id);

            var vehicleIds = dsXe.Select(v => v.Id).Distinct().ToList();
            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Filter("vehicle_id", Operator.In, vehicleIds).Get()).Models ?? new List<PhieuTiepNhan>();
            var receiptIds = receipts.Select(r => r.Id).Distinct().ToList();

            var orders = (await App.DB._client.From<PhieuSuaChua>().Filter("service_receipt_id", Operator.In, receiptIds).Get()).Models ?? new List<PhieuSuaChua>();
            var orderIds = orders.Select(o => o.Id).Distinct().ToList();

            var payments = (await App.DB._client.From<PhieuThuTien>().Filter("repair_order_id", Operator.In, orderIds).Get()).Models ?? new List<PhieuThuTien>();

            return dsXe.Select((v, index) => {
                customerMap.TryGetValue(v.KhachHangId ?? "", out var c);
                brandMap.TryGetValue(v.HieuXeId ?? "", out var b);

                decimal tienNo = v.TinhTienNo(orders, payments);

                return new TraCuuXeRow {
                    STT = index + 1,
                    BienSo = v.BienSo,
                    HieuXe = b?.TenHieuXe ?? "",
                    ChuXe = c?.HoTen ?? "",
                    TienNo = tienNo
                };
            }).ToList();
        }

        public async Task<TraCuuDashBoardStats> GetDashboardStatsAsync()
        {
            var temp = new TiepNhanViewModel();
            return await temp.GetDashboardStatsAsync();
        }

        public List<XeItemViewModel> MapToUiModel(List<TraCuuXeRow> data)
        {
            return data.Select((item, index) => new XeItemViewModel
            {
                STT = (index + 1).ToString(),
                BienSo = item.BienSo,
                HieuXe = item.HieuXe,
                ChuXe = item.ChuXe,
                TienNo = item.TienNo.ToString("N0") + " đ",
                Color = item.TienNo > 0 ? "#D93025" : "#222222"
            }).ToList();
        }
    }

    public class XeItemViewModel
    {
        public string STT { get; set; }
        public string BienSo { get; set; }
        public string HieuXe { get; set; }
        public string ChuXe { get; set; }
        public string TienNo { get; set; }
        public string Color { get; set; }
    }
}
