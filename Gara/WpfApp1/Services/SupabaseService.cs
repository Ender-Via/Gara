using Supabase;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class SupabaseService
    {
        // Biến này để lưu giữ kết nối, dùng đi dùng lại
        public Supabase.Client _client;

        public async Task InitializeAsync()
        {
            var url = "https://klsafpqatqpohiqykoud.supabase.co";

            var key = "sb_publishable_3F5w0GD2L9c_sNGD3wfpvQ_ZCGOLM-1";

            // Cấu hình kết nối
            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            // Khởi tạo kết nối
            _client = new Supabase.Client(url, key, options);
            await _client.InitializeAsync();
        }
        public async Task<bool> LuuTiepNhanXeAsync(string tenKhach, string sdt, string diaChi, string bienSo, string tenHieuXe, DateTime ngayTiepNhan)
        {
            try
            {
                var brandResponse = await _client.From<CarBrand>()
                                                 .Filter("brand_name", Postgrest.Constants.Operator.Equals, tenHieuXe)
                                                 .Get();
                var brand = brandResponse.Models.FirstOrDefault();

                if (brand == null)
                {
                    MessageBox.Show($"Lỗi: Không tìm thấy hiệu xe '{tenHieuXe}' trong database!");
                    return false;
                }

                
                var newCustomer = new Customer
                {
                    FullName = tenKhach,
                    Phone = sdt,
                    Address = diaChi
                };
                var customerResponse = await _client.From<Customer>().Insert(newCustomer);
                var customerId = customerResponse.Models.First().Id;

               
                var newVehicle = new Vehicle
                {
                    LicensePlate = bienSo,
                    CustomerId = customerId,
                    CarBrandId = brand.Id
                };
                var vehicleResponse = await _client.From<Vehicle>().Insert(newVehicle);
                var vehicleId = vehicleResponse.Models.First().Id;
                
                var newReceipt = new ServiceReceipt
                {
                    ReceptionDate = ngayTiepNhan,
                    VehicleId = vehicleId
                };
                await _client.From<ServiceReceipt>().Insert(newReceipt);

                return true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu DB: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> LuuPhieuSuaChuaAsync(DateTime ngaySuaChua, IEnumerable<RepairOrderDetail> danhSachChiTiet)
        {

            var danhSachThucTe = danhSachChiTiet.Where(x => !string.IsNullOrEmpty(x.Content) || x.PartId != null).ToList();

            if (!danhSachThucTe.Any())
            {
                throw new Exception("Bảng chi tiết đang trống!");
            }


            decimal tongTien = danhSachThucTe.Sum(x => x.LineTotal ?? 0);


            var newOrder = new RepairOrder
            {

                RepairDate = ngaySuaChua,
                TotalAmount = tongTien
            };


            var orderResponse = await _client.From<RepairOrder>().Insert(newOrder);
            var insertedOrder = orderResponse.Models.FirstOrDefault();

            if (insertedOrder == null)
                throw new Exception("Không tạo được phiếu");

            foreach (var item in danhSachThucTe)
            {
                item.RepairOrderId = insertedOrder.Id;
            }

            await _client.From<RepairOrderDetail>().Insert(danhSachThucTe);

            return true; 
        }
    }
}