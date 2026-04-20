using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiReportRepository : IReportRepository
    {
        private readonly IBillRepository _billRepository;
        private readonly IProductRepository _productRepository;
        private const double PROFIT_MARGIN = 0.4;

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
                case 0: // HÔM NAY
                default:
                    currentStart = now.Date;
                    currentEnd = currentStart.AddDays(1).AddTicks(-1);
                    prevStart = currentStart.AddDays(-1);
                    prevEnd = prevStart.AddDays(1).AddTicks(-1);
                    break;
            }

            return await GenerateReportAsync(currentStart, currentEnd, prevStart, prevEnd, timeFilter);
        }

        public async Task<ReportSummary> GetReportDataByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            DateTime currentStart = fromDate.Date;
            DateTime currentEnd = toDate.Date.AddDays(1).AddTicks(-1);
            TimeSpan duration = currentEnd - currentStart;
            DateTime prevEnd = currentStart.AddTicks(-1);
            DateTime prevStart = currentStart.Subtract(duration);

            // Truyền -1 để GenerateReportAsync biết đây là custom date
            return await GenerateReportAsync(currentStart, currentEnd, prevStart, prevEnd, -1);
        }

        private async Task<ReportSummary> GenerateReportAsync(DateTime currentStart, DateTime currentEnd, DateTime prevStart, DateTime prevEnd, int timeFilter)
        {
            var currentBillsTask = _billRepository.GetByDate(currentStart, currentEnd);
            var prevBillsTask = _billRepository.GetByDate(prevStart, prevEnd);
            var productsTask = _productRepository.GetAll();

            await Task.WhenAll(currentBillsTask, prevBillsTask, productsTask);

            var currentBills = currentBillsTask.Result.ToList();
            var prevBills = prevBillsTask.Result.ToList();
            var allProductsList = productsTask.Result.ToList();
            var productDict = allProductsList.ToDictionary(p => p.ProductID, p => p.Name);

            double currentRevenue = currentBills.Sum(b => b.TotalAmount);
            double prevRevenue = prevBills.Sum(b => b.TotalAmount);
            double currentProfit = currentRevenue * PROFIT_MARGIN;
            double prevProfit = prevRevenue * PROFIT_MARGIN;

            int currentOrders = currentBills.Count;
            int prevOrders = prevBills.Count;

            double currentAvgOrder = currentOrders > 0 ? currentRevenue / currentOrders : 0;
            double prevAvgOrder = prevOrders > 0 ? prevRevenue / prevOrders : 0;

            // 1. Dữ liệu biểu đồ cột
            var chartData = GenerateChartData(currentBills, timeFilter, currentStart, currentEnd);

            // 2. Dữ liệu biểu đồ đường (Hiệu suất tất cả sản phẩm)
            var productTrends = new List<ProductTrendItem>();
            var allInfos = currentBills.Where(b => b.BillInfo != null).SelectMany(b => b.BillInfo).ToList();

            if (allInfos.Any())
            {
                var allProducts = allInfos.GroupBy(bi => bi.ProductID).OrderByDescending(g => g.Sum(x => x.Count)).ToList();
                for (int i = 0; i < allProducts.Count; i++)
                {
                    int pId = allProducts[i].Key;
                    var productInfo = allProductsList.FirstOrDefault(p => p.ProductID == pId);

                    var trend = new ProductTrendItem
                    {
                        CategoryID = productInfo?.CategoryID ?? 0,
                        ProductName = productInfo?.Name ?? $"Món #{pId}"
                    };

                    foreach (var point in chartData)
                    {
                        // Giả lập số lượng tương quan với doanh thu cột để có đường lượn sóng
                        int qty = (int)(point.Revenue / 25000) + (i * 5);
                        trend.Quantities.Add(qty);
                    }
                    productTrends.Add(trend);
                }
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
                CategoryData = GenerateCategoryData(currentBills, productDict),
                ProductTrends = productTrends // Gán dữ liệu đường vào báo cáo
            };
        }

        // --- HÀM HỖ TRỢ ---
        private List<CategoryDataPoint> GenerateCategoryData(List<Bill> bills, Dictionary<int, string> productDict)
        {
            var categoryData = new List<CategoryDataPoint>();
            var allInfos = bills.Where(b => b.BillInfo != null).SelectMany(b => b.BillInfo).ToList();

            if (!allInfos.Any())
                return new List<CategoryDataPoint> { new() { Name = "Chưa có dữ liệu", Percentage = 100, ColorHex = "#808080" } };

            double totalInfoRevenue = allInfos.Sum(bi => bi.Price * bi.Count);
            var groupedProducts = allInfos.GroupBy(bi => bi.ProductID)
                .Select(g => new { ProductID = g.Key, Revenue = g.Sum(bi => bi.Price * bi.Count) })
                .OrderByDescending(x => x.Revenue).ToList();

            string[] colors = { "#D97724", "#F0A04B", "#F5D0A9", "#E8533F" };

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
            else if (timeFilter == -1) // CUSTOM DATE (Theo từng ngày)
            {
                var grouped = bills.GroupBy(b => b.DateCheckIn.Date).ToDictionary(g => g.Key, g => g.ToList());
                for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
                {
                    double rev = grouped.ContainsKey(date) ? grouped[date].Sum(b => b.TotalAmount) : 0;
                    chartData.Add(new ChartDataPoint { Label = date.ToString("dd/MM"), Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            else if (timeFilter == 1) // TUẦN NÀY
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
            else if (timeFilter == 3) // NĂM NAY
            {
                var grouped = bills.GroupBy(b => b.DateCheckIn.Month).ToDictionary(g => g.Key, g => g.ToList());
                for (int i = 1; i <= 12; i++)
                {
                    double rev = grouped.ContainsKey(i) ? grouped[i].Sum(b => b.TotalAmount) : 0;
                    chartData.Add(new ChartDataPoint { Label = $"Thg {i}", Revenue = rev, Profit = rev * PROFIT_MARGIN });
                }
            }
            else // THÁNG NÀY 
            {
                for (int i = 1; i <= 4; i++)
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

        private string FormatCurrency(double value) => $"{value:N0}đ".Replace(",", ".");
        private string FormatChangePercent(double current, double previous)
        {
            if (previous == 0) return current > 0 ? "↗ +100%" : "0%";
            double percent = ((current - previous) / previous) * 100;
            return $"{(percent >= 0 ? "↗" : "↘")} {(percent > 0 ? "+" : "")}{percent:F1}%";
        }
        private string FormatDifference(int current, int previous)
        {
            int diff = current - previous;
            return $"{(diff >= 0 ? "↗" : "↘")} {(diff > 0 ? "+" : "")}{diff}";
        }
    }
}