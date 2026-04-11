using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class ReportSummary
    {
        public string TotalRevenue { get; set; } = string.Empty;
        public string RevenueChange { get; set; } = string.Empty;
        public string EstimatedProfit { get; set; } = string.Empty;
        public string ProfitChange { get; set; } = string.Empty;
        public string AverageOrder { get; set; } = string.Empty;
        public string AverageOrderChange { get; set; } = string.Empty;
        public string TotalOrders { get; set; } = string.Empty;
        public string OrdersChange { get; set; } = string.Empty;

        // Dữ liệu thô cho biểu đồ
        public List<ChartDataPoint> ChartData { get; set; } = new();
        public List<CategoryDataPoint> CategoryData { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Revenue { get; set; }
        public double Profit { get; set; }
    }

    public class CategoryDataPoint
    {
        public string Name { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public string ColorHex { get; set; } = string.Empty;
    }
}
