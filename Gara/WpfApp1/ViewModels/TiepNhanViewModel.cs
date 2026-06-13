using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;
using static Postgrest.Constants;

namespace WpfApp1.ViewModels
{
    public class TiepNhanViewModel
    {
        public async Task<List<HieuXe>> GetCarBrandsAsync()
        {
            var response = await App.DB._client.From<HieuXe>().Get();
            return response.Models ?? new List<HieuXe>();
        }

        public async Task<TraCuuDashBoardStats> GetDashboardStatsAsync()
        {
            var stats = new TraCuuDashBoardStats();

            // [DECENTRALIZATION]: ViewModel điều phối thống kê thô
            var vehicles = (await App.DB._client.From<Xe>().Get()).Models ?? new List<Xe>();
            stats.TongSoXe = vehicles.Count;

            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Get()).Models ?? new List<PhieuTiepNhan>();
            var orders = (await App.DB._client.From<PhieuSuaChua>().Get()).Models ?? new List<PhieuSuaChua>();
            var payments = (await App.DB._client.From<PhieuThuTien>().Get()).Models ?? new List<PhieuThuTien>();

            var receiptWithOrders = orders.Select(o => o.PhieuTiepNhanId).Distinct().ToList();
            stats.DangSuaChua = receipts.Count(r => !receiptWithOrders.Contains(r.Id));

            decimal tongTienSua = orders.Sum(o => o.TongTien ?? 0);
            decimal tongDaThu = payments.Sum(p => p.SoTienThu ?? 0);
            stats.TongNo = tongTienSua - tongDaThu;

            var today = DateTime.Today;
            var startDateStr = today.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var todayReceipts = await App.DB._client.From<PhieuTiepNhan>()
                .Filter("reception_date", Operator.GreaterThanOrEqual, startDateStr)
                .Get();
            stats.LuotXeTrongNgay = todayReceipts.Models?.Count ?? 0;

            var quyDinh = (await App.DB._client.From<QuyDinh>().Get()).Models?.FirstOrDefault();
            stats.MaxDailyVehicles = quyDinh?.SoXeTiepNhanToiDa ?? 30;

            if (receipts.Count > 0)
                stats.HieuSuatSuaChua = (decimal)receiptWithOrders.Count / receipts.Count * 100m;

            return stats;
        }

        public async Task<List<RecentReceiptDTO>> GetRecentReceiptsAsync(int limit = 5)
        {
            // [DECENTRALIZATION]: Luồng lấy DTO gần đây
            var receipts = (await App.DB._client.From<PhieuTiepNhan>()
                .Order("reception_date", Ordering.Descending)
                .Limit(limit)
                .Get()).Models ?? new List<PhieuTiepNhan>();

            if (!receipts.Any()) return new List<RecentReceiptDTO>();

            var vehicleIds = receipts.Select(r => r.XeId).Distinct().ToList();
            var vehicles = (await App.DB._client.From<Xe>().Filter("id", Operator.In, vehicleIds).Get()).Models ?? new List<Xe>();

            var customerIds = vehicles.Select(v => v.KhachHangId).Distinct().ToList();
            var brandIds = vehicles.Select(v => v.HieuXeId).Distinct().ToList();

            var customers = (await App.DB._client.From<KhachHang>().Filter("id", Operator.In, customerIds).Get()).Models ?? new List<KhachHang>();
            var brands = (await App.DB._client.From<HieuXe>().Filter("id", Operator.In, brandIds).Get()).Models ?? new List<HieuXe>();

            var vehicleMap = vehicles.ToDictionary(v => v.Id);
            var customerMap = customers.ToDictionary(c => c.Id);
            var brandMap = brands.ToDictionary(b => b.Id);

            return receipts.Select(r =>
            {
                vehicleMap.TryGetValue(r.XeId ?? string.Empty, out var v);
                if (v != null)
                {
                    customerMap.TryGetValue(v.KhachHangId ?? string.Empty, out var c);
                    brandMap.TryGetValue(v.HieuXeId ?? string.Empty, out var b);
                    v.KhachHang = c;
                    v.HieuXe = b;
                    r.Xe = v;
                }
                return r.TaoDTOHienThi(); // Gọi behavior của Entity
            }).ToList();
        }

        public async Task<bool> LuuTiepNhanXeAsync(string tenKhach, string sdt, string diaChi, string bienSo, string tenHieuXe, DateTime ngayTiepNhan)
        {
            var quyDinh = (await App.DB._client.From<QuyDinh>().Get()).Models?.FirstOrDefault() ?? new QuyDinh { SoXeTiepNhanToiDa = 30 };

            var startUtc = ngayTiepNhan.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var endUtc = ngayTiepNhan.Date.AddDays(1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            var receiptCount = (await App.DB._client.From<PhieuTiepNhan>()
                .Filter("reception_date", Operator.GreaterThanOrEqual, startUtc)
                .Filter("reception_date", Operator.LessThan, endUtc)
                .Get()).Models?.Count ?? 0;

            if (!quyDinh.ChoPhepTiepNhan(receiptCount)) return false;

            var brand = (await App.DB._client.From<HieuXe>().Filter("brand_name", Operator.Equals, tenHieuXe).Get()).Models?.FirstOrDefault();
            if (brand == null) return false;

            var customer = new KhachHang { HoTen = tenKhach, DienThoai = sdt, DiaChi = diaChi };
            var customerRes = await App.DB._client.From<KhachHang>().Insert(customer);
            var insertedCustomer = customerRes.Models.First();

            var xe = new Xe { BienSo = bienSo };
            xe.GanChuXe(insertedCustomer);
            xe.GanHieuXe(brand);
            var xeRes = await App.DB._client.From<Xe>().Insert(xe);
            var insertedXe = xeRes.Models.First();

            var phieu = new PhieuTiepNhan { NgayTiepNhan = ngayTiepNhan.ToUniversalTime(), XeId = insertedXe.Id };
            await App.DB._client.From<PhieuTiepNhan>().Insert(phieu);

            return true;
        }
    }
}
