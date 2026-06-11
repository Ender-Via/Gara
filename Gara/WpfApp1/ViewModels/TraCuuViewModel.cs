using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models.DTOs;

namespace WpfApp1.ViewModels
{
    public class TraCuuViewModel
    {
        public async Task<List<TraCuuXeRow>> TraCuuXeAsync(string bienSoFilter)
        {
            return await App.DB.TraCuuXeAsync(bienSoFilter);
        }

        public async Task<TraCuuDashBoardStats> GetDashboardStatsAsync()
        {
            return await App.DB.GetDashboardStatsAsync();
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
