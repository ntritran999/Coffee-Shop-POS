using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiReportRepository : IReportRepository
    {
        private readonly IBillRepository _billRepository;
        private readonly IProductRepository _productRepository;

        private const double PROFIT_MARGIN = 0.4; // Giả định lợi nhuận 40% trên doanh thu

        public ApiReportRepository(IBillRepository billRepository, IProductRepository productRepository)
        {
            _billRepository = billRepository;
            _productRepository = productRepository;
        }

        public async Task<ReportSummary> GetReportData(int timeFilter)
        {
            DateTime now = DateTime.Now;
            DateTime currentStart, currentEnd, prevStart, prevEnd;

            switch (timeFilter)
            {
                case 1: // TUẦN NÀY
                    int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
                    currentStart = now.Date.AddDays(-diff);
                    currentEnd = currentStart.AddDays(7).AddTicks(-1);
                    prevStart = currentStart.AddDays(-7);
                    prevEnd = currentStart.AddTicks(-1);
                    break;
                case 2: // THÁNG NÀY
                    currentStart = new DateTime(now.Year, now.Month, 1);
                    currentEnd = currentStart.AddMonths(1).AddTicks(-1);
                    prevStart = currentStart.AddMonths(-1);
                    prevEnd = currentStart.AddTicks(-1);
                    break;
                case 3: // NĂM NAY
                    currentStart = new DateTime(now.Year, 1, 1);
                    currentEnd = new DateTime(now.Year, 12, 31, 23, 59, 59);
                    prevStart = new DateTime(now.Year - 1, 1, 1);
                    prevEnd = new DateTime(now.Year - 1, 12, 31, 23, 59, 59);
                    break;
                case 0: // HÔM NAY (Mặc định)
                default:
                    currentStart = now.Date;
                    currentEnd = currentStart.AddDays(1).AddTicks(-1);
                    prevStart = currentStart.AddDays(-1);
                    prevEnd = prevStart.AddDays(1).AddTicks(-1);
                    break;
            }

            // Gọi API đồng thời để tối ưu thời gian chờ
            var currentBillsTask = _billRepository.GetByDate(currentStart, currentEnd);
            var prevBillsTask = _billRepository.GetByDate(prevStart, prevEnd);
            var productsTask = _productRepository.GetAll();

            // Chờ tất cả dữ liệu tải xong
            await Task.WhenAll(currentBillsTask, prevBillsTask, productsTask);

            var currentBills = currentBillsTask.Result.ToList();
            var prevBills = prevBillsTask.Result.ToList();

            // Chuyển Product list thành Dictionary để tìm tên món siêu nhanh: Dict[ProductID] = Name
            var productDict = productsTask.Result.ToDictionary(p => p.ProductID, p => p.Name);

            // Tính toán số liệu tổng quan
            double currentRevenue = currentBills.Sum(b => b.TotalAmount);
            double prevRevenue = prevBills.Sum(b => b.TotalAmount);

            double currentProfit = currentRevenue * PROFIT_MARGIN;
            double prevProfit = prevRevenue * PROFIT_MARGIN;

            int currentOrders = currentBills.Count;
            int prevOrders = prevBills.Count;

            double currentAvgOrder = currentOrders > 0 ? currentRevenue / currentOrders : 0;
            double prevAvgOrder = prevOrders > 0 ? prevRevenue / prevOrders : 0;

            // Lắp ráp báo cáo
            return new ReportSummary
            {
                TotalRevenue = FormatCurrency(currentRevenue),
                RevenueChange = FormatChangePercent(currentRevenue, prevRevenue),
                EstimatedProfit = FormatCurrency(currentProfit),
                ProfitChange = FormatChangePercent(currentProfit, prevProfit),
                AverageOrder = FormatCurrency(currentAvgOrder),
                AverageOrderChange = FormatChangePercent(currentAvgOrder, prevAvgOrder),
                TotalOrders = currentOrders.ToString("N0").Replace(",", "."),
                OrdersChange = FormatDifference(currentOrders, prevOrders),
                ChartData = GenerateChartData(currentBills, timeFilter, currentStart, currentEnd),
                CategoryData = GenerateCategoryData(currentBills, productDict)
            };
        }

        public async Task<ReportSummary> GetReportDataByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            // Đảm bảo lấy từ đầu ngày fromDate đến cuối ngày toDate
            DateTime currentStart = fromDate.Date;
            DateTime currentEnd = toDate.Date.AddDays(1).AddTicks(-1);

            // Tính kỳ trước để so sánh (có độ dài tương đương)
            TimeSpan duration = currentEnd - currentStart;
            DateTime prevEnd = currentStart.AddTicks(-1);
            DateTime prevStart = currentStart.Subtract(duration);

            var currentBillsTask = _billRepository.GetByDate(currentStart, currentEnd);
            var prevBillsTask = _billRepository.GetByDate(prevStart, prevEnd);
            var productsTask = _productRepository.GetAll();

            await Task.WhenAll(currentBillsTask, prevBillsTask, productsTask);

            var currentBills = currentBillsTask.Result.ToList();
            var prevBills = prevBillsTask.Result.ToList();
            var productDict = productsTask.Result.ToDictionary(p => p.ProductID, p => p.Name);

            double currentRevenue = currentBills.Sum(b => b.TotalAmount);
            double prevRevenue = prevBills.Sum(b => b.TotalAmount);
            double currentProfit = currentRevenue * PROFIT_MARGIN;
            double prevProfit = prevRevenue * PROFIT_MARGIN;

            int currentOrders = currentBills.Count;
            int prevOrders = prevBills.Count;

            double currentAvgOrder = currentOrders > 0 ? currentRevenue / currentOrders : 0;
            double prevAvgOrder = prevOrders > 0 ? prevRevenue / prevOrders : 0;

            // --- Vẽ biểu đồ cột theo từng ngày trong khoảng thời gian đã chọn ---
            var chartData = new List<ChartDataPoint>();
            var grouped = currentBills.GroupBy(b => b.DateCheckIn.Date).ToDictionary(g => g.Key, g => g.ToList());

            for (DateTime date = currentStart.Date; date <= currentEnd.Date; date = date.AddDays(1))
            {
                double rev = grouped.ContainsKey(date) ? grouped[date].Sum(b => b.TotalAmount) : 0;
                chartData.Add(new ChartDataPoint { Label = date.ToString("dd/MM"), Revenue = rev, Profit = rev * PROFIT_MARGIN });
            }

            return new ReportSummary
            {
                TotalRevenue = FormatCurrency(currentRevenue),
                RevenueChange = FormatChangePercent(currentRevenue, prevRevenue),
                EstimatedProfit = FormatCurrency(currentProfit),
                ProfitChange = FormatChangePercent(currentProfit, prevProfit),
                AverageOrder = FormatCurrency(currentAvgOrder),
                AverageOrderChange = FormatChangePercent(currentAvgOrder, prevAvgOrder),
                TotalOrders = currentOrders.ToString("N0").Replace(",", "."),
                OrdersChange = FormatDifference(currentOrders, prevOrders),
                ChartData = chartData,
                CategoryData = GenerateCategoryData(currentBills, productDict) 
            };
        }

        // ---------------- CÁC HÀM XỬ LÝ DỮ LIỆU & BIỂU ĐỒ ----------------

        private List<CategoryDataPoint> GenerateCategoryData(List<Bill> bills, Dictionary<int, string> productDict)
        {
            var categoryData = new List<CategoryDataPoint>();

            // Lấy tất cả chi tiết món (BillInfo) từ danh sách hóa đơn
            var allInfos = bills.Where(b => b.BillInfo != null)
                                .SelectMany(b => b.BillInfo)
                                .ToList();

            if (!allInfos.Any())
            {
                return new List<CategoryDataPoint>
                {
                    new() { Name = "Chưa có dữ liệu", Percentage = 100, ColorHex = "#808080" }
                };
            }

            // Tính tổng tiền các món ăn bán được
            double totalInfoRevenue = allInfos.Sum(bi => bi.Price * bi.Count);

            // Nhóm theo ProductID và tính doanh thu từng món
            var groupedProducts = allInfos.GroupBy(bi => bi.ProductID)
                .Select(g => new
                {
                    ProductID = g.Key,
                    Revenue = g.Sum(bi => bi.Price * bi.Count)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            string[] colors = { "#D97724", "#F0A04B", "#F5D0A9", "#E8533F" };

            // Lấy Top 3 món bán chạy nhất
            for (int i = 0; i < groupedProducts.Count && i < 3; i++)
            {
                var item = groupedProducts[i];
                string productName = productDict.TryGetValue(item.ProductID, out var name) ? name : $"Món #{item.ProductID}";

                categoryData.Add(new CategoryDataPoint
                {
                    Name = productName,
                    Percentage = totalInfoRevenue > 0 ? (int)Math.Round((item.Revenue / totalInfoRevenue) * 100) : 0,
                    ColorHex = colors[i]
                });
            }

            // Các món còn lại gom vào mục "Khác"
            if (groupedProducts.Count > 3)
            {
                double otherRevenue = groupedProducts.Skip(3).Sum(x => x.Revenue);
                categoryData.Add(new CategoryDataPoint
                {
                    Name = "Khác",
                    Percentage = totalInfoRevenue > 0 ? (int)Math.Round((otherRevenue / totalInfoRevenue) * 100) : 0,
                    ColorHex = colors[3]
                });
            }

            return categoryData;
        }

        private List<ChartDataPoint> GenerateChartData(List<Bill> bills, int timeFilter, DateTime start, DateTime end)
        {
            var chartData = new List<ChartDataPoint>();

            if (timeFilter == 0) // HÔM NAY (Theo giờ)
            {
                var grouped = bills.GroupBy(b => b.DateCheckIn.Hour).ToDictionary(g => g.Key, g => g.ToList());
                for (int i = 6; i <= 22; i += 2)
                {
                    double rev = grouped.ContainsKey(i) ? grouped[i].Sum(b => b.TotalAmount) : 0;
                    chartData.Add(new ChartDataPoint { Label = $"{i:D2}:00", Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            else if (timeFilter == 1) // TUẦN NÀY (Theo ngày)
            {
                var grouped = bills.GroupBy(b => b.DateCheckIn.Date).ToDictionary(g => g.Key, g => g.ToList());
                string[] dayLabels = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "CN" };
                for (int i = 0; i < 7; i++)
                {
                    var date = start.AddDays(i).Date;
                    double rev = grouped.ContainsKey(date) ? grouped[date].Sum(b => b.TotalAmount) : 0;
                    chartData.Add(new ChartDataPoint { Label = dayLabels[i], Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            else if (timeFilter == 3) // NĂM NAY (Theo tháng)
            {
                var grouped = bills.GroupBy(b => b.DateCheckIn.Month).ToDictionary(g => g.Key, g => g.ToList());
                for (int i = 1; i <= 12; i++)
                {
                    double rev = grouped.ContainsKey(i) ? grouped[i].Sum(b => b.TotalAmount) : 0;
                    chartData.Add(new ChartDataPoint { Label = $"Thg {i}", Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            else // THÁNG NÀY (Theo tuần)
            {
                for (int i = 1; i <= 4; i++) // Chia tạm 4 tuần
                {
                    var weekStart = start.AddDays((i - 1) * 7);
                    var weekEnd = i == 4 ? end : weekStart.AddDays(7).AddTicks(-1);

                    var weekBills = bills.Where(b => b.DateCheckIn >= weekStart && b.DateCheckIn <= weekEnd).ToList();
                    double rev = weekBills.Sum(b => b.TotalAmount);

                    chartData.Add(new ChartDataPoint { Label = $"Tuần {i}", Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            return chartData;
        }

        // ---------------- CÁC HÀM FORMAT TEXT ----------------

        private string FormatCurrency(double value)
        {
            return $"{value:N0}đ".Replace(",", ".");
        }

        private string FormatChangePercent(double current, double previous)
        {
            if (previous == 0) return current > 0 ? "↗ +100%" : "0%";
            double percent = ((current - previous) / previous) * 100;
            string arrow = percent >= 0 ? "↗" : "↘";
            string sign = percent > 0 ? "+" : "";
            return $"{arrow} {sign}{percent:F1}%";
        }

        private string FormatDifference(int current, int previous)
        {
            int diff = current - previous;
            string arrow = diff >= 0 ? "↗" : "↘";
            string sign = diff > 0 ? "+" : "";
            return $"{arrow} {sign}{diff}";
        }
    }
}
