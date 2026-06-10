using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using WpfApp1.Models.DTOs;

namespace WpfApp1.ViewModels
{
    public class PhieuThuViewModel
    {
        public async Task<string> GetNextPaymentCodeAsync()
        {
            return await App.DB.GetNextPaymentCodeAsync();
        }

        public async Task<List<RecentPaymentReceiptRow>> GetRecentPaymentReceiptsAsync()
        {
            return await App.DB.GetRecentPaymentReceiptsAsync();
        }

        public async Task<PaymentDebtSummary> GetVehiclePaymentSummaryAsync(string licensePlate)
        {
            return await App.DB.GetVehiclePaymentSummaryAsync(licensePlate);
        }

        public async Task<PaymentReceipt> CreatePaymentReceiptAsync(string licensePlate, decimal soTienThu, DateTime ngayThu, string ghiChu)
        {
            return await App.DB.CreatePaymentReceiptAsync(licensePlate, soTienThu, ngayThu, ghiChu);
        }
    }
}
