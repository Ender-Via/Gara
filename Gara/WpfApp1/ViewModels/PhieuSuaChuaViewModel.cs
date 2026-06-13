using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.Entities;
using static Postgrest.Constants;

namespace WpfApp1.ViewModels
{
    public class PhieuSuaChuaViewModel
    {
        public async Task<List<string>> GetLicensePlatesAsync()
        {
            var response = await App.DB._client.From<Xe>().Get();
            return response.Models?.Select(x => x.BienSo).ToList() ?? new List<string>();
        }

        public async Task<List<VatTuPhuTung>> GetPartsAsync()
        {
            var response = await App.DB._client.From<VatTuPhuTung>()
                .Order("part_name", Ordering.Ascending)
                .Get();
            return response.Models ?? new List<VatTuPhuTung>();
        }

        public async Task<List<TienCong>> GetLaborsAsync()
        {
            var response = await App.DB._client.From<TienCong>()
                .Order("labor_name", Ordering.Ascending)
                .Get();
            return response.Models ?? new List<TienCong>();
        }

        public async Task<bool> LuuPhieuSuaChuaAsync(string licensePlate, DateTime ngaySuaChua, IEnumerable<ChiTietPhieuSuaChua> danhSachChiTiet)
        {
            var danhSachThucTe = danhSachChiTiet.Where(x => !string.IsNullOrEmpty(x.NoiDung) || x.VatTuPhuTungId != null).ToList();

            if (!danhSachThucTe.Any())
                throw new Exception("Bảng chi tiết đang trống!");

            // [DECENTRALIZATION]: ViewModel trực tiếp điều phối quy trình lưu trữ
            var vehicleRes = await App.DB._client.From<Xe>().Filter("license_plate", Operator.Equals, licensePlate).Get();
            var vehicle = vehicleRes.Models.FirstOrDefault();
            if (vehicle == null) throw new Exception("Không tìm thấy xe");

            var receiptRes = await App.DB._client.From<PhieuTiepNhan>()
                .Filter("vehicle_id", Operator.Equals, vehicle.Id)
                .Order("reception_date", Ordering.Descending)
                .Get();
            var receipt = receiptRes.Models.FirstOrDefault();
            if (receipt == null) throw new Exception("Xe chưa được tiếp nhận");

            // Khởi tạo thực thể và giao phó logic tính toán cho nó
            var newOrder = new PhieuSuaChua
            {
                NgaySuaChua = DateTime.SpecifyKind(ngaySuaChua, DateTimeKind.Local).ToUniversalTime(),
                PhieuTiepNhanId = receipt.Id
            };
            newOrder.TinhTongTien(danhSachThucTe);

            var orderResponse = await App.DB._client.From<PhieuSuaChua>().Insert(newOrder);
            var insertedOrder = orderResponse.Models.FirstOrDefault();

            if (insertedOrder == null)
                throw new Exception("Không tạo được phiếu");

            foreach (var item in danhSachThucTe)
            {
                item.PhieuSuaChuaId = insertedOrder.Id;
                item.TinhThanhTienChiTiet();
            }

            await App.DB._client.From<ChiTietPhieuSuaChua>().Insert(danhSachThucTe);

            // Xử lý xuất kho thông qua Entity
            var danhSachGiaoDich = insertedOrder.TaoGiaoDichXuatKho(danhSachThucTe);
            if (danhSachGiaoDich.Any())
            {
                await App.DB._client.From<GiaoDichKho>().Insert(danhSachGiaoDich);

                foreach (var gd in danhSachGiaoDich)
                {
                    var partRes = await App.DB._client.From<VatTuPhuTung>().Filter("id", Operator.Equals, gd.VatTuPhuTungId).Get();
                    var part = partRes.Models.FirstOrDefault();
                    if (part != null)
                    {
                        part.CapNhatSoLuong(gd.SoLuong ?? 0, "XUAT");
                        await App.DB._client.From<VatTuPhuTung>().Update(part);
                    }
                }
            }

            return true;
        }

        public decimal CalculateTotal(IEnumerable<ChiTietPhieuSuaChua> details)
        {
            return details.Sum(x => x.ThanhTien ?? 0);
        }
    }
}
