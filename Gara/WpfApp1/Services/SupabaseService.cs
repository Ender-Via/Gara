using Supabase;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using static Postgrest.Constants;

namespace WpfApp1.Services
{
    public class SupabaseService
    {
        public Supabase.Client _client;

        public async Task InitializeAsync()
        {
            var url = "https://klsafpqatqpohiqykoud.supabase.co";
            var key = "sb_publishable_3F5w0GD2L9c_sNGD3wfpvQ_ZCGOLM-1";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(url, key, options);
            await _client.InitializeAsync();
        }

        public async Task<List<RecentReceiptDTO>> GetRecentReceiptsAsync(int limit = 5)
        {
            try
            {
                var receiptsResponse = await _client.From<ServiceReceipt>()
                    .Order("reception_date", Ordering.Descending)
                    .Limit(limit)
                    .Get();

                var receipts = receiptsResponse.Models?.ToList() ?? new List<ServiceReceipt>();
                if (!receipts.Any())
                    return new List<RecentReceiptDTO>();

                var vehicleIds = receipts.Select(r => r.VehicleId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
                var vehicles = new List<Vehicle>();
                if (vehicleIds.Any())
                {
                    var vehiclesResponse = await _client.From<Vehicle>()
                        .Filter("id", Operator.In, vehicleIds)
                        .Get();
                    vehicles = vehiclesResponse.Models?.ToList() ?? new List<Vehicle>();
                }

                var customerIds = vehicles.Select(v => v.CustomerId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
                var brandIds = vehicles.Select(v => v.CarBrandId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();

                var customers = new List<Customer>();
                if (customerIds.Any())
                {
                    var customersResponse = await _client.From<Customer>()
                        .Filter("id", Operator.In, customerIds)
                        .Get();
                    customers = customersResponse.Models?.ToList() ?? new List<Customer>();
                }

                var brands = new List<CarBrand>();
                if (brandIds.Any())
                {
                    var brandsResponse = await _client.From<CarBrand>()
                        .Filter("id", Operator.In, brandIds)
                        .Get();
                    brands = brandsResponse.Models?.ToList() ?? new List<CarBrand>();
                }

                var vehicleMap = vehicles.ToDictionary(v => v.Id);
                var customerMap = customers.ToDictionary(c => c.Id);
                var brandMap = brands.ToDictionary(b => b.Id);

                return receipts.Select(r =>
                {
                    vehicleMap.TryGetValue(r.VehicleId ?? string.Empty, out var v);
                    customerMap.TryGetValue(v?.CustomerId ?? string.Empty, out var c);
                    brandMap.TryGetValue(v?.CarBrandId ?? string.Empty, out var b);

                    var utcInstant = DateTime.SpecifyKind(r.ReceptionDate, DateTimeKind.Utc);
                    var displayTime = utcInstant.AddHours(0);

                    return new RecentReceiptDTO
                    {
                        TenKhach = c?.FullName ?? "Không rõ",
                        BienSo = v?.LicensePlate ?? "Không rõ",
                        HieuXe = b?.BrandName ?? string.Empty,
                        ThoiGian = displayTime.ToString("dd/MM/yyyy HH:mm")
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải tiếp nhận gần đây: " + ex.Message);
                return new List<RecentReceiptDTO>();
            }
        }

        public async Task<bool> LuuTiepNhanXeAsync(string tenKhach, string sdt, string diaChi, string bienSo, string tenHieuXe, DateTime ngayTiepNhan)
        {
            try
            {
                var regulations = await GetRegulationsAsync();
                var maxDailyVehicles = regulations?.MaxDailyVehicles ?? 30;

                var startLocal = ngayTiepNhan.Date;
                var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local).ToUniversalTime();
                var endUtc = DateTime.SpecifyKind(startLocal.AddDays(1), DateTimeKind.Local).ToUniversalTime();
                var startDate = startUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endDate = endUtc.ToString("yyyy-MM-ddTHH:mm:ssZ");

                var receiptCountResponse = await _client.From<ServiceReceipt>()
                    .Filter("reception_date", Operator.GreaterThanOrEqual, startDate)
                    .Filter("reception_date", Operator.LessThan, endDate)
                    .Get();

                if ((receiptCountResponse.Models?.Count ?? 0) >= maxDailyVehicles)
                {
                    MessageBox.Show($"Lỗi: Mỗi ngày chỉ tiếp nhận tối đa {maxDailyVehicles} xe.");
                    return false;
                }

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

                // Store reception date in UTC to avoid timezone issues when querying by day range
                var newReceipt = new ServiceReceipt
                {
                    ReceptionDate = DateTime.SpecifyKind(ngayTiepNhan, DateTimeKind.Local).ToUniversalTime(),
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

        public async Task<SystemRegulation> GetRegulationsAsync()
        {
            var response = await _client.From<SystemRegulation>().Get();
            return response.Models.FirstOrDefault();
        }

        public async Task<bool> SaveRegulationsAsync(int maxCarBrands, int maxDailyVehicles, int maxParts, int maxLabors)
        {
            var existing = await GetRegulationsAsync();
            if (existing == null)
            {
                var newRegulation = new SystemRegulation
                {
                    MaxCarBrands = maxCarBrands,
                    MaxDailyVehicles = maxDailyVehicles,
                    MaxParts = maxParts,
                    MaxLabors = maxLabors
                };

                await _client.From<SystemRegulation>().Insert(newRegulation);
                return true;
            }

            existing.MaxCarBrands = maxCarBrands;
            existing.MaxDailyVehicles = maxDailyVehicles;
            existing.MaxParts = maxParts;
            existing.MaxLabors = maxLabors;

            await _client.From<SystemRegulation>().Update(existing);
            return true;
        }

        public async Task<bool> LuuPhieuSuaChuaAsync(DateTime ngaySuaChua, IEnumerable<RepairOrderDetail> danhSachChiTiet)
        {
            var danhSachThucTe = danhSachChiTiet.Where(x => !string.IsNullOrEmpty(x.Content) || x.PartId != null).ToList();

            if (!danhSachThucTe.Any())
                throw new Exception("Bảng chi tiết đang trống!");

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

        public async Task<PaymentDebtSummary> GetVehiclePaymentSummaryAsync(string licensePlate)
        {
            var summary = new PaymentDebtSummary();

            var vehicleResponse = await _client.From<Vehicle>()
                .Filter("license_plate", Operator.Equals, licensePlate)
                .Get();

            var vehicle = vehicleResponse.Models.FirstOrDefault();
            if (vehicle == null) return summary;

            summary.Vehicle = vehicle;

            if (!string.IsNullOrWhiteSpace(vehicle.CustomerId))
            {
                var customerResponse = await _client.From<Customer>()
                    .Filter("id", Operator.Equals, vehicle.CustomerId)
                    .Get();
                summary.Customer = customerResponse.Models.FirstOrDefault();
            }

            var receiptsResponse = await _client.From<ServiceReceipt>()
                .Filter("vehicle_id", Operator.Equals, vehicle.Id)
                .Get();
            var receiptIds = receiptsResponse.Models.Select(r => r.Id).ToList();

            if (!receiptIds.Any()) return summary;

            var ordersResponse = await _client.From<RepairOrder>()
                .Filter("service_receipt_id", Operator.In, receiptIds)
                .Get();
            var orders = ordersResponse.Models;
            var orderIds = orders.Select(o => o.Id).ToList();

            summary.TotalRepairAmount = orders.Sum(o => o.TotalAmount ?? 0);
            summary.LatestRepairOrderId = orders
                .OrderByDescending(o => o.RepairDate)
                .FirstOrDefault()?.Id ?? string.Empty;

            if (orderIds.Any())
            {
                var paymentsResponse = await _client.From<PaymentReceipt>()
                    .Filter("repair_order_id", Operator.In, orderIds)
                    .Get();
                summary.TotalPaidAmount = paymentsResponse.Models.Sum(p => p.AmountReceived ?? 0);
            }

            summary.CurrentDebt = summary.TotalRepairAmount - summary.TotalPaidAmount;
            return summary;
        }

        public async Task<(Vehicle? vehicle, decimal currentDebt)> GetVehicleDebtAsync(string licensePlate)
        {
            var summary = await GetVehiclePaymentSummaryAsync(licensePlate);
            return (summary.Vehicle, summary.CurrentDebt);
        }

        public async Task<List<RecentPaymentReceiptRow>> GetRecentPaymentReceiptsAsync(int limit = 5)
        {
            var paymentsResponse = await _client.From<PaymentReceipt>()
                .Order("receipt_date", Ordering.Descending)
                .Limit(limit)
                .Get();
            var payments = paymentsResponse.Models?.ToList() ?? new List<PaymentReceipt>();

            if (!payments.Any())
                return new List<RecentPaymentReceiptRow>();

            var orderIds = payments
                .Select(p => p.RepairOrderId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var orders = new List<RepairOrder>();
            if (orderIds.Any())
            {
                var ordersResponse = await _client.From<RepairOrder>()
                    .Filter("id", Operator.In, orderIds)
                    .Get();
                orders = ordersResponse.Models?.ToList() ?? new List<RepairOrder>();
            }

            var receiptIds = orders
                .Select(o => o.ServiceReceiptId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var receipts = new List<ServiceReceipt>();
            if (receiptIds.Any())
            {
                var receiptsResponse = await _client.From<ServiceReceipt>()
                    .Filter("id", Operator.In, receiptIds)
                    .Get();
                receipts = receiptsResponse.Models?.ToList() ?? new List<ServiceReceipt>();
            }

            var vehicleIds = receipts
                .Select(r => r.VehicleId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var vehicles = new List<Vehicle>();
            if (vehicleIds.Any())
            {
                var vehiclesResponse = await _client.From<Vehicle>()
                    .Filter("id", Operator.In, vehicleIds)
                    .Get();
                vehicles = vehiclesResponse.Models?.ToList() ?? new List<Vehicle>();
            }

            var customerIds = vehicles
                .Select(v => v.CustomerId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            var customers = new List<Customer>();
            if (customerIds.Any())
            {
                var customersResponse = await _client.From<Customer>()
                    .Filter("id", Operator.In, customerIds)
                    .Get();
                customers = customersResponse.Models?.ToList() ?? new List<Customer>();
            }

            var orderMap = orders.ToDictionary(o => o.Id);
            var receiptMap = receipts.ToDictionary(r => r.Id);
            var vehicleMap = vehicles.ToDictionary(v => v.Id);
            var customerMap = customers.ToDictionary(c => c.Id);

            return payments.Select((payment, index) =>
            {
                orderMap.TryGetValue(payment.RepairOrderId ?? string.Empty, out var order);
                receiptMap.TryGetValue(order?.ServiceReceiptId ?? string.Empty, out var receipt);
                vehicleMap.TryGetValue(receipt?.VehicleId ?? string.Empty, out var vehicle);
                customerMap.TryGetValue(vehicle?.CustomerId ?? string.Empty, out var customer);

                return new RecentPaymentReceiptRow
                {
                    MaPhieu = GetPaymentReceiptDisplayCode(payment, index),
                    ChuXe = customer?.FullName ?? "Không rõ",
                    BienSo = vehicle?.LicensePlate ?? "Không rõ",
                    NgayThu = payment.ReceiptDate.ToString("dd/MM/yyyy"),
                    SoTien = $"{payment.AmountReceived ?? 0:N0}",
                    TrangThai = "THÀNH CÔNG"
                };
            }).ToList();
        }

        public async Task<PaymentReceipt> CreatePaymentReceiptAsync(string licensePlate, decimal soTienThu, DateTime ngayThu, string ghiChu)
        {
            if (soTienThu <= 0)
                throw new Exception("Số tiền thu không hợp lệ!");

            var summary = await GetVehiclePaymentSummaryAsync(licensePlate);
            if (summary.Vehicle == null)
                throw new Exception("Không tìm thấy xe này trong hệ thống!");

            if (string.IsNullOrWhiteSpace(summary.LatestRepairOrderId))
                throw new Exception("Xe này chưa có phiếu sửa chữa để lập phiếu thu.");

            if (soTienThu > summary.CurrentDebt)
                throw new Exception($"Số tiền thu ({soTienThu:N0}) không được vượt quá số tiền nợ ({summary.CurrentDebt:N0}).");

            var newPayment = new PaymentReceipt
            {
                RepairOrderId = summary.LatestRepairOrderId,
                AmountReceived = soTienThu,
                ReceiptDate = ngayThu,
                Note = ghiChu
            };

            var response = await _client.From<PaymentReceipt>().Insert(newPayment);
            return response.Models?.FirstOrDefault() ?? newPayment;
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

        public static string GetPaymentReceiptDisplayCode(PaymentReceipt payment, int index)
        {
            const string notePrefix = "Phiếu thu ";
            var note = payment.Note?.Trim();

            if (!string.IsNullOrWhiteSpace(note) && note.StartsWith(notePrefix, StringComparison.OrdinalIgnoreCase))
            {
                var code = note[notePrefix.Length..].Trim();
                var firstSpaceIndex = code.IndexOf(' ');
                return firstSpaceIndex > 0 ? code[..firstSpaceIndex] : code;
            }

            return $"PT{payment.ReceiptDate:ddMMyy}{(index + 1):D3}";
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

        public async Task<TraCuuDashBoardStats> GetDashboardStatsAsync()
        {
            var stats = new TraCuuDashBoardStats();

            try
            {
                var vehiclesResponse = await _client.From<Vehicle>().Get();
                var vehicles = vehiclesResponse.Models?.ToList() ?? new List<Vehicle>();
                stats.TongSoXe = vehicles.Count;

                var receiptsResponse = await _client.From<ServiceReceipt>().Get();
                var receipts = receiptsResponse.Models?.ToList() ?? new List<ServiceReceipt>();

                var ordersResponse = await _client.From<RepairOrder>().Get();
                var orders = ordersResponse.Models?.ToList() ?? new List<RepairOrder>();

                var paymentsResponse = await _client.From<PaymentReceipt>().Get();
                var payments = paymentsResponse.Models?.ToList() ?? new List<PaymentReceipt>();

                var receiptWithOrders = orders.Select(o => o.ServiceReceiptId).Distinct().ToList();
                stats.DangSuaChua = receipts.Count(r => !receiptWithOrders.Contains(r.Id));
                if (stats.DangSuaChua < 0) stats.DangSuaChua = 0;

                decimal tongTienSua = orders.Sum(o => o.TotalAmount ?? 0);
                decimal tongDaThu = payments.Sum(p => p.AmountReceived ?? 0);
                stats.TongNo = tongTienSua - tongDaThu;

                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var startDateStr = today.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endDateStr = tomorrow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                var todayReceipts = await _client.From<ServiceReceipt>()
                    .Filter("reception_date", Operator.GreaterThanOrEqual, startDateStr)
                    .Filter("reception_date", Operator.LessThan, endDateStr)
                    .Get();
                stats.LuotXeTrongNgay = todayReceipts.Models?.Count ?? 0;

                var regulations = await GetRegulationsAsync();
                stats.MaxDailyVehicles = regulations?.MaxDailyVehicles ?? 30;

                if (receipts.Count > 0)
                {
                    stats.HieuSuatSuaChua = (decimal)receiptWithOrders.Count / receipts.Count * 100m;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dashboard: " + ex.Message);
            }

            return stats;
        }

        // Quy định Window

        public async Task<Models.SystemRegulation> GetSystemRegulationsAsync()
        {
            var response = await _client.From<Models.SystemRegulation>().Get();
            return response.Models.FirstOrDefault();
        }

        public async Task<QuyDinhDashboardStats> GetQuyDinhDashboardStatsAsync()
        {
            var stats = new QuyDinhDashboardStats();

            var regulations = await GetSystemRegulationsAsync();
            stats.HieuXeToiDa = regulations?.MaxCarBrands ?? 10;
            stats.LuotXeToiDaNgay = regulations?.MaxDailyVehicles ?? 30;
            stats.DichVuToiDa = (regulations?.MaxParts ?? 100) + (regulations?.MaxLabors ?? 20);

            var vehiclesResponse = await _client.From<Vehicle>().Get();
            var vehicles = vehiclesResponse.Models?.ToList() ?? new List<Vehicle>();
            stats.HieuXeHienTai = vehicles
                .Select(v => v.CarBrandId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .Count();

            var receiptsResponse = await _client.From<ServiceReceipt>().Get();
            var receipts = receiptsResponse.Models?.ToList() ?? new List<ServiceReceipt>();
            if (receipts.Any())
            {
                var minDate = receipts.Min(r => r.ReceptionDate).Date;
                var maxDate = receipts.Max(r => r.ReceptionDate).Date;
                var totalDays = Math.Max(1, (maxDate - minDate).Days + 1);
                stats.LuotXeTrungBinhNgay = Math.Round((decimal)receipts.Count / totalDays, 1);
            }
            else
            {
                stats.LuotXeTrungBinhNgay = 0;
            }

            var orderCount = (await _client.From<RepairOrder>().Get()).Models?.Count ?? 0;
            stats.DichVuDangNiemYet = orderCount;

            return stats;
        }

        public async Task<List<Models.SystemRegulationHistory>> GetSystemRegulationHistoryAsync()
        {
            var response = await _client.From<Models.SystemRegulationHistory>()
                .Order("changed_at", Postgrest.Constants.Ordering.Descending)
                .Get();
            return response.Models ?? new List<Models.SystemRegulationHistory>();
        }

        public async Task UpsertSystemRegulationAsync(
            int maxBrands, int maxVehicles, int maxParts, int maxLabors,
            SystemRegulation oldReg)
        {
            var reg = new Models.SystemRegulation
            {
                Id = oldReg?.Id ?? Guid.NewGuid().ToString(),
                MaxCarBrands = maxBrands,
                MaxDailyVehicles = maxVehicles,
                MaxParts = maxParts,
                MaxLabors = maxLabors
            };

            await _client.From<Models.SystemRegulation>()
                .Upsert(reg, new Postgrest.QueryOptions { OnConflict = "id" });

            var changes = new[]
            {
                ("QD1_MAX_BRANDS",       oldReg?.MaxCarBrands.ToString(),     maxBrands.ToString()),
                ("QD1_MAX_CARS_PER_DAY", oldReg?.MaxDailyVehicles.ToString(), maxVehicles.ToString()),
                ("QD2_MAX_PART_TYPES",   oldReg?.MaxParts.ToString(),         maxParts.ToString()),
                ("QD2_MAX_LABOR_TYPES",  oldReg?.MaxLabors.ToString(),        maxLabors.ToString()),
            };

            foreach (var (key, oldVal, newVal) in changes)
            {
                if (oldVal == newVal) continue;
                var history = new Models.SystemRegulationHistory
                {
                    RegulationKey = key,
                    OldValue = oldVal ?? "0",
                    NewValue = newVal,
                    ChangedBy = "Quản trị viên",
                    ChangedAt = DateTime.UtcNow
                };
                await _client.From<Models.SystemRegulationHistory>().Insert(history);
            }
        }
    }
}