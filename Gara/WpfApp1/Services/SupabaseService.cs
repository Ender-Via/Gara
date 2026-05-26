using Supabase;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using static Postgrest.Constants;

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

        public async Task<(Vehicle vehicle, decimal currentDebt)> GetVehicleDebtAsync(string licensePlate)
        {
            
            var vehicleResponse = await _client.From<Vehicle>()
                .Filter("license_plate", Operator.Equals, licensePlate)
                .Get();

            var vehicle = vehicleResponse.Models.FirstOrDefault();
            if (vehicle == null) return (null, 0);

           
            var receiptsResponse = await _client.From<ServiceReceipt>()
                .Filter("vehicle_id", Operator.Equals, vehicle.Id)
                .Get();
            var receiptIds = receiptsResponse.Models.Select(r => r.Id).ToList();

            if (!receiptIds.Any()) return (vehicle, 0);

            
            var ordersResponse = await _client.From<RepairOrder>()
                .Filter("service_receipt_id", Operator.In, receiptIds)
                .Get();
            var orders = ordersResponse.Models;
            var orderIds = orders.Select(o => o.Id).ToList();

            decimal totalInvoiced = orders.Sum(o => o.TotalAmount ?? 0);

            
            decimal totalPaid = 0;
            if (orderIds.Any())
            {
                var paymentsResponse = await _client.From<PaymentReceipt>()
                    .Filter("repair_order_id", Operator.In, orderIds)
                    .Get();
                totalPaid = paymentsResponse.Models.Sum(p => p.AmountReceived ?? 0);
            }

            return (vehicle, totalInvoiced - totalPaid);
        }

       
        public async Task<bool> LuuPhieuThuAsync(string repairOrderId, decimal soTienThu, DateTime ngayThu, string ghiChu)
        {
            var newPayment = new PaymentReceipt
            {
                RepairOrderId = repairOrderId,
                AmountReceived = soTienThu,
                ReceiptDate = ngayThu,
                Note = ghiChu
            };

            await _client.From<PaymentReceipt>().Insert(newPayment);
            return true;
        }

        public async Task<string> GetNextPaymentCodeAsync()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            string startDate = today.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endDate = tomorrow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            var response = await _client.From<PaymentReceipt>()
                .Filter("receipt_date", Operator.GreaterThanOrEqual, startDate)
                .Filter("receipt_date", Operator.LessThan, endDate)
                .Get();

            int count = (response.Models?.Count ?? 0) + 1;

            // PT + ddmmyy + STT (3 số)
            return $"PT{DateTime.Now:ddMMyy}{count:D3}";
        }
        public async Task<List<TraCuuXeRow>> TraCuuXeAsync(string bienSoFilter)
        {
            try
            {
                var vehicleResponse = string.IsNullOrWhiteSpace(bienSoFilter)
                ? await _client.From<Vehicle>().Get()
                : await _client.From<Vehicle>()
                .Filter("license_plate", Operator.ILike, $"%{bienSoFilter}%")
                .Get();
                var vehicles = vehicleResponse.Models?.ToList() ?? new List<Vehicle>();
                if (!vehicles.Any())
                    return new List<TraCuuXeRow>();

                // Lấy khách hàng & hãng xe
                var customerIds = vehicles.Select(v => v.CustomerId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var brandIds = vehicles.Select(v => v.CarBrandId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

                var customersResponse = await _client.From<Customer>()
                    .Filter("id", Operator.In, customerIds)
                    .Get();
                var brandsResponse = await _client.From<CarBrand>()
                    .Filter("id", Operator.In, brandIds)
                    .Get();

                var customerMap = customersResponse.Models?.ToDictionary(x => x.Id) ?? new Dictionary<string, Customer>();
                var brandMap = brandsResponse.Models?.ToDictionary(x => x.Id) ?? new Dictionary<string, CarBrand>();

                // Lấy phiếu tiếp nhận -> lệnh sửa -> phiếu thu để tính nợ
                var vehicleIds = vehicles.Select(v => v.Id).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var receiptsResponse = await _client.From<ServiceReceipt>()
                    .Filter("vehicle_id", Operator.In, vehicleIds)
                    .Get();
                var receipts = receiptsResponse.Models?.ToList() ?? new List<ServiceReceipt>();

                var receiptIds = receipts.Select(r => r.Id).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var ordersResponse = await _client.From<RepairOrder>()
                    .Filter("service_receipt_id", Operator.In, receiptIds)
                    .Get();
                var orders = ordersResponse.Models?.ToList() ?? new List<RepairOrder>();

                var orderIds = orders.Select(o => o.Id).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var paymentsResponse = await _client.From<PaymentReceipt>()
                    .Filter("repair_order_id", Operator.In, orderIds)
                    .Get();
                var payments = paymentsResponse.Models?.ToList() ?? new List<PaymentReceipt>();

                var result = new List<TraCuuXeRow>();
                int stt = 1;

                foreach (var v in vehicles)
                {
                    var customer = customerMap.TryGetValue(v.CustomerId ?? "", out var c) ? c : null;
                    var brand = brandMap.TryGetValue(v.CarBrandId ?? "", out var b) ? b : null;

                    var vehicleReceipts = receipts.Where(r => r.VehicleId == v.Id).Select(r => r.Id).ToList();
                    var vehicleOrders = orders.Where(o => vehicleReceipts.Contains(o.ServiceReceiptId)).ToList();
                    var vehicleOrderIds = vehicleOrders.Select(o => o.Id).ToList();

                    decimal tongTienSua = vehicleOrders.Sum(o => o.TotalAmount ?? 0);
                    decimal tongDaThu = payments.Where(p => vehicleOrderIds.Contains(p.RepairOrderId)).Sum(p => p.AmountReceived ?? 0);
                    decimal tienNo = tongTienSua - tongDaThu;

                    result.Add(new TraCuuXeRow
                    {
                        STT = stt++,
                        BienSo = v.LicensePlate,
                        HieuXe = brand?.BrandName ?? "",
                        ChuXe = customer?.FullName ?? "",
                        TienNo = tienNo
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tra cứu xe: " + ex.Message);
                return new List<TraCuuXeRow>();
            }
        }
            
        //Quy định Window

        public class SystemRegulation
        {
            public string RegulationKey { get; set; } = "";
            public string RegulationValue { get; set; } = "";
            public string Description { get; set; } = "";
        }

        public class SystemRegulationHistory
        {
            public DateTime ChangedAt { get; set; }
            public string RegulationKey { get; set; } = "";
            public string OldValue { get; set; } = "";
            public string NewValue { get; set; } = "";
            public string ChangedBy { get; set; } = "";
        }

        public async Task<List<SystemRegulation>> GetSystemRegulationsAsync()
        {
            return await Task.FromResult(new List<SystemRegulation>());
        }

        public async Task<List<SystemRegulationHistory>> GetSystemRegulationHistoryAsync()
        {
            return await Task.FromResult(new List<SystemRegulationHistory>());
        }

        public async Task UpsertSystemRegulationAsync(string key, string value, string description)
        {
            await Task.CompletedTask;
        }
    }
}