using ClosedXML.Excel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class BaoCaoWindow : Window
    {
        public List<BaoCaoDoanhThuRow> BaoCaoDoanhThu { get; set; }
        public List<BaoCaoTonKhoRow> BaoCaoTonVatTu { get; set; }

        public BaoCaoWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void BtnXemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang.SelectedItem == null || cbNam.SelectedItem == null)
            {
                MessageBox.Show("Chọn tháng và năm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int month = int.Parse(((ComboBoxItem)cbThang.SelectedItem).Content.ToString());
            int year = int.Parse(((ComboBoxItem)cbNam.SelectedItem).Content.ToString());

            BtnXemBaoCao.IsEnabled = false;

            try
            {
                BaoCaoDoanhThu = await App.DB.GetBaoCaoDoanhSoAsync(month, year);
                BaoCaoTonVatTu = await App.DB.GetBaoCaoTonKhoAsync(month, year);

                dgvDoanhSo.ItemsSource = BaoCaoDoanhThu;
                dgvTonKho.ItemsSource = BaoCaoTonVatTu;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải báo cáo: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnXemBaoCao.IsEnabled = true;
            }
        }
        private void BtnXuatExcel_Click(object sender, RoutedEventArgs e)
        {
            if (BaoCaoDoanhThu == null || BaoCaoTonVatTu == null || cbThang.SelectedItem == null || cbNam.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn thời gian và bấm 'Xem báo cáo' trước khi xuất Excel!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string thang = ((ComboBoxItem)cbThang.SelectedItem).Content.ToString();
            string nam = ((ComboBoxItem)cbNam.SelectedItem).Content.ToString();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = $"BaoCao_DinhKy_Thang_{thang}_{nam}.xlsx",
                Title = "Chọn nơi lưu file Báo Cáo"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        // ==========================================
                        // TRANG TÍNH 1: BÁO CÁO DOANH SỐ
                        // ==========================================
                        var ws1 = workbook.Worksheets.Add("Doanh Số Hiệu Xe");
                        ws1.ShowGridLines = true; // FIX: Bản 0.105.0 gọi trực tiếp từ Worksheet

                        // Tiêu đề báo cáo (Sử dụng gán thuộc tính trực tiếp)
                        var cellA1 = ws1.Cell("A1");
                        cellA1.Value = "BÁO CÁO DOANH SỐ THEO HIỆU XE";
                        cellA1.Style.Font.Bold = true;
                        cellA1.Style.Font.FontSize = 16;
                        cellA1.Style.Font.FontColor = XLColor.FromHtml("#1E3A8A");

                        var cellA2 = ws1.Cell("A2");
                        cellA2.Value = $"Tháng {thang} / Năm {nam}";
                        cellA2.Style.Font.Italic = true;
                        cellA2.Style.Font.FontColor = XLColor.FromHtml("#64748B");

                        // Tiêu đề cột
                        string[] headers1 = { "STT", "Hiệu Xe", "Số Lượt Sửa", "Thành Tiền", "Tỉ Lệ (%)" };
                        for (int i = 0; i < headers1.Length; i++)
                        {
                            var cell = ws1.Cell(4, i + 1);
                            cell.Value = headers1[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Font.FontColor = XLColor.White;
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        }
                        ws1.Row(4).Height = 26;

                        // Điền dữ liệu
                        int startRow1 = 5;
                        for (int i = 0; i < BaoCaoDoanhThu.Count; i++)
                        {
                            int currentRow = startRow1 + i;
                            var item = BaoCaoDoanhThu[i];

                            ws1.Cell(currentRow, 1).Value = item.STT;
                            ws1.Cell(currentRow, 2).Value = item.HieuXe;
                            ws1.Cell(currentRow, 3).Value = item.SoLuotSua;
                            ws1.Cell(currentRow, 4).Value = item.ThanhTien;
                            ws1.Cell(currentRow, 5).Value = (double)(item.TiLe / 100);

                            ws1.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws1.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws1.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).NumberFormat.Format = "#,##0";
                            ws1.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).NumberFormat.Format = "#,##0";
                            ws1.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).NumberFormat.Format = "0.0%";

                            for (int col = 1; col <= 5; col++)
                            {
                                ws1.Cell(currentRow, col).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                                ws1.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
                                if (currentRow % 2 == 0)
                                {
                                    ws1.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                                }
                            }
                            ws1.Row(currentRow).Height = 20;
                        }

                        // Dòng tổng cộng
                        int totalRow1 = startRow1 + BaoCaoDoanhThu.Count;
                        ws1.Cell(totalRow1, 2).Value = "Tổng cộng";
                        ws1.Cell(totalRow1, 2).Style.Font.Bold = true;

                        ws1.Cell(totalRow1, 3).FormulaA1 = $"=SUM(C5:C{totalRow1 - 1})";
                        ws1.Cell(totalRow1, 3).Style.Font.Bold = true;
                        ws1.Cell(totalRow1, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        ws1.Cell(totalRow1, 3).Style.NumberFormat.Format = "#,##0";

                        ws1.Cell(totalRow1, 4).FormulaA1 = $"=SUM(D5:D{totalRow1 - 1})";
                        ws1.Cell(totalRow1, 4).Style.Font.Bold = true;
                        ws1.Cell(totalRow1, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws1.Cell(totalRow1, 4).Style.NumberFormat.Format = "#,##0";

                        ws1.Cell(totalRow1, 5).FormulaA1 = $"=SUM(E5:E{totalRow1 - 1})";
                        ws1.Cell(totalRow1, 5).Style.Font.Bold = true;
                        ws1.Cell(totalRow1, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        ws1.Cell(totalRow1, 5).Style.NumberFormat.Format = "0.0%";

                        for (int col = 1; col <= 5; col++)
                        {
                            var cell = ws1.Cell(totalRow1, col);
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
                        }
                        ws1.Row(totalRow1).Height = 24;
                        ws1.Columns(1, 5).AdjustToContents();

                        // ==========================================
                        // TRANG TÍNH 2: BÁO CÁO TỒN KHO
                        // ==========================================
                        var ws2 = workbook.Worksheets.Add("Tồn Kho Vật Tư");
                        ws2.ShowGridLines = true; // FIX: Bản 0.105.0 gọi trực tiếp từ Worksheet

                        var cellTitle2 = ws2.Cell("A1");
                        cellTitle2.Value = "BÁO CÁO TỒN VẬT TƯ PHỤ TÙNG";
                        cellTitle2.Style.Font.Bold = true;
                        cellTitle2.Style.Font.FontSize = 16;
                        cellTitle2.Style.Font.FontColor = XLColor.FromHtml("#1E3A8A");

                        var cellSub2 = ws2.Cell("A2");
                        cellSub2.Value = $"Tháng {thang} / Năm {nam}";
                        cellSub2.Style.Font.Italic = true;
                        cellSub2.Style.Font.FontColor = XLColor.FromHtml("#64748B");

                        string[] headers2 = { "STT", "Vật Tư Phụ Tùng", "Tồn Đầu", "Phát Sinh", "Tồn Cuối" };
                        for (int i = 0; i < headers2.Length; i++)
                        {
                            var cell = ws2.Cell(4, i + 1);
                            cell.Value = headers2[i];
                            cell.Style.Font.Bold = true;
                            cell.Style.Font.FontColor = XLColor.White;
                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
                            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                        }
                        ws2.Row(4).Height = 26;

                        int startRow2 = 5;
                        for (int i = 0; i < BaoCaoTonVatTu.Count; i++)
                        {
                            int currentRow = startRow2 + i;
                            var item = BaoCaoTonVatTu[i];

                            ws2.Cell(currentRow, 1).Value = item.STT;
                            ws2.Cell(currentRow, 2).Value = item.VatTuPhuTung;
                            ws2.Cell(currentRow, 3).Value = item.TonDau;
                            ws2.Cell(currentRow, 4).Value = item.PhatSinh;
                            ws2.Cell(currentRow, 5).Value = item.TonCuoi;

                            ws2.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws2.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws2.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).NumberFormat.Format = "#,##0";
                            ws2.Cell(currentRow, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).NumberFormat.Format = "#,##0";
                            ws2.Cell(currentRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).NumberFormat.Format = "#,##0";

                            for (int col = 1; col <= 5; col++)
                            {
                                ws2.Cell(currentRow, col).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                                ws2.Cell(currentRow, col).Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
                                if (currentRow % 2 == 0)
                                {
                                    ws2.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                                }
                            }
                            ws2.Row(currentRow).Height = 20;
                        }
                        ws2.Columns(1, 5).AdjustToContents();

                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Xuất file báo cáo định kỳ Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi tạo file Excel: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}