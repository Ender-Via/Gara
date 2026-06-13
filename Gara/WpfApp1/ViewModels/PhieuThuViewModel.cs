using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;
using static Postgrest.Constants;

namespace WpfApp1.ViewModels
{
    public class PhieuThuViewModel
    {
        public async Task<string> GetNextPaymentCodeAsync()
        {
            var today = DateTime.Now.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var tomorrow = DateTime.Now.Date.AddDays(1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            var response = await App.DB._client.From<PhieuThuTien>()
                .Filter("receipt_date", Operator.GreaterThanOrEqual, today)
                .Filter("receipt_date", Operator.LessThan, tomorrow)
                .Get();

            int count = (response.Models?.Count ?? 0) + 1;
            return $"PT{DateTime.Now:ddMMyy}{count:D3}";
        }

        public async Task<List<RecentPaymentReceiptRow>> GetRecentPaymentReceiptsAsync()
        {
            var payments = (await App.DB._client.From<PhieuThuTien>()
                .Order("receipt_date", Ordering.Descending)
                .Limit(5)
                .Get()).Models ?? new List<PhieuThuTien>();

            if (!payments.Any()) return new List<RecentPaymentReceiptRow>();

            // [DECENTRALIZATION]: ViewModel điều phối quy trình join thô
            var orderIds = payments.Select(p => p.PhieuSuaChuaId).Distinct().ToList();
            var orders = (await App.DB._client.From<PhieuSuaChua>().Filter("id", Operator.In, orderIds).Get()).Models ?? new List<PhieuSuaChua>();

            var receiptIds = orders.Select(o => o.PhieuTiepNhanId).Distinct().ToList();
            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Filter("id", Operator.In, receiptIds).Get()).Models ?? new List<PhieuTiepNhan>();

            var vehicleIds = receipts.Select(r => r.XeId).Distinct().ToList();
            var vehicles = (await App.DB._client.From<Xe>().Filter("id", Operator.In, vehicleIds).Get()).Models ?? new List<Xe>();

            var customerIds = vehicles.Select(v => v.KhachHangId).Distinct().ToList();
            var customers = (await App.DB._client.From<KhachHang>().Filter("id", Operator.In, customerIds).Get()).Models ?? new List<KhachHang>();

            var orderMap = orders.ToDictionary(o => o.Id);
            var receiptMap = receipts.ToDictionary(r => r.Id);
            var vehicleMap = vehicles.ToDictionary(v => v.Id);
            var customerMap = customers.ToDictionary(c => c.Id);

            return payments.Select((p, index) =>
            {
                orderMap.TryGetValue(p.PhieuSuaChuaId ?? string.Empty, out var o);
                receiptMap.TryGetValue(o?.PhieuTiepNhanId ?? string.Empty, out var r);
                vehicleMap.TryGetValue(r?.XeId ?? string.Empty, out var v);
                customerMap.TryGetValue(v?.KhachHangId ?? string.Empty, out var c);

                return new RecentPaymentReceiptRow
                {
                    MaPhieu = $"PT{p.NgayThuTien:ddMMyy}{(index + 1):D3}",
                    ChuXe = c?.HoTen ?? "Không rõ",
                    BienSo = v?.BienSo ?? "Không rõ",
                    NgayThu = p.NgayThuTien.ToString("dd/MM/yyyy"),
                    SoTien = $"{p.SoTienThu ?? 0:N0}",
                    TrangThai = "THÀNH CÔNG"
                };
            }).ToList();
        }

        public async Task<PaymentDebtSummary> GetVehiclePaymentSummaryAsync(string licensePlate)
        {
            var xe = (await App.DB._client.From<Xe>().Filter("license_plate", Operator.Equals, licensePlate).Get()).Models?.FirstOrDefault();
            if (xe == null) return new PaymentDebtSummary();

            var customer = (await App.DB._client.From<KhachHang>().Filter("id", Operator.Equals, xe.KhachHangId).Get()).Models?.FirstOrDefault();
            var receipts = (await App.DB._client.From<PhieuTiepNhan>().Filter("vehicle_id", Operator.Equals, xe.Id).Get()).Models ?? new List<PhieuTiepNhan>();
            var receiptIds = receipts.Select(r => r.Id).ToList();

            var orders = (await App.DB._client.From<PhieuSuaChua>().Filter("service_receipt_id", Operator.In, receiptIds).Get()).Models ?? new List<PhieuSuaChua>();
            var orderIds = orders.Select(o => o.Id).ToList();

            decimal totalPaid = 0;
            if (orderIds.Any())
            {
                totalPaid = (await App.DB._client.From<PhieuThuTien>().Filter("repair_order_id", Operator.In, orderIds).Get()).Models?.Sum(p => p.SoTienThu ?? 0) ?? 0;
            }

            decimal totalRepair = orders.Sum(o => o.TongTien ?? 0);

            return new PaymentDebtSummary
            {
                Vehicle = xe,
                Customer = customer,
                TotalRepairAmount = totalRepair,
                TotalPaidAmount = totalPaid,
                CurrentDebt = totalRepair - totalPaid,
                LatestRepairOrderId = orders.OrderByDescending(o => o.NgaySuaChua).FirstOrDefault()?.Id ?? string.Empty
            };
        }

        public async Task<PhieuThuTien> CreatePaymentReceiptAsync(string licensePlate, decimal soTienThu, DateTime ngayThu, string ghiChu)
        {
            var summary = await GetVehiclePaymentSummaryAsync(licensePlate);
            var payment = new PhieuThuTien
            {
                PhieuSuaChuaId = summary.LatestRepairOrderId,
                SoTienThu = soTienThu,
                NgayThuTien = ngayThu,
                GhiChu = ghiChu
            };
            var res = await App.DB._client.From<PhieuThuTien>().Insert(payment);
            return res.Models.First();
        }
    }
}
