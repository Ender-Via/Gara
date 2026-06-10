using System;

namespace WpfApp1.Models.DTOs
{
    // DTO dùng để hiển thị tra cứu xe (không map DB)
    public class TraCuuXeRow
    {
        public int STT { get; set; }
        public string BienSo { get; set; }
        public string HieuXe { get; set; }
        public string ChuXe { get; set; }
        public decimal TienNo { get; set; }
    }

    public class TraCuuDashBoardStats
    {
        public int TongSoXe { get; set; }
        public int DangSuaChua { get; set; }
        public decimal TongNo { get; set; }
        public decimal HieuSuatSuaChua { get; set; }
        public int LuotXeTrongNgay { get; set; }
        public int MaxDailyVehicles { get; set; }
    }

    public class QuyDinhDashboardStats
    {
        public int HieuXeHienTai { get; set; }
        public int HieuXeToiDa { get; set; }
        public decimal LuotXeTrungBinhNgay { get; set; }
        public int LuotXeToiDaNgay { get; set; }
        public int DichVuDangNiemYet { get; set; }
        public int DichVuToiDa { get; set; }
    }

    public class BaoCaoDoanhThuRow
    {
        public int STT { get; set; }
        public string HieuXe { get; set; }
        public int SoLuotSua { get; set; }
        public decimal ThanhTien { get; set; }
        public double TiLe { get; set; }
    }

    public class BaoCaoTonKhoRow
    {
        public int STT { get; set; }
        public string VatTuPhuTung { get; set; }
        public decimal TonDau { get; set; }
        public decimal PhatSinh { get; set; }
        public decimal TonCuoi { get; set; }
    }

    // DTO cho Tiếp nhận gần đây
    public class RecentReceiptDTO
    {
        public string TenKhach { get; set; }
        public string BienSo { get; set; }
        public string HieuXe { get; set; }
        public string ThoiGian { get; set; }
    }

    public class PaymentDebtSummary
    {
        public Entities.Vehicle? Vehicle { get; set; }
        public Entities.Customer? Customer { get; set; }
        public decimal TotalRepairAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal CurrentDebt { get; set; }
        public string LatestRepairOrderId { get; set; } = string.Empty;
    }

    public class RecentPaymentReceiptRow
    {
        public string MaPhieu { get; set; } = string.Empty;
        public string ChuXe { get; set; } = string.Empty;
        public string BienSo { get; set; } = string.Empty;
        public string NgayThu { get; set; } = string.Empty;
        public string SoTien { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "THÀNH CÔNG";
    }
}
