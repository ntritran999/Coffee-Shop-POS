//using Client.Models;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Client.Repositories
//{
//    public class MockReportRepository : IReportRepository
//    {
//        public ReportSummary GetReportData(int timeFilter)
//        {
//            return timeFilter switch
//            {
//                // 0: NGÀY HÔM NAY (Mốc thời gian theo giờ)
//                0 => new ReportSummary
//                {
//                    TotalRevenue = "4.250.000đ",
//                    RevenueChange = "↗ +12.5%",
//                    EstimatedProfit = "1.800.000đ",
//                    ProfitChange = "↗ +15.2%",
//                    AverageOrder = "45.000đ",
//                    AverageOrderChange = "↗ +3.0%",
//                    TotalOrders = "94",
//                    OrdersChange = "↗ +8",
//                    ChartData = new List<ChartDataPoint>
//                    {
//                        new() { Label = "08:00", Revenue = 400000, Profit = 150000 },
//                        new() { Label = "10:00", Revenue = 850000, Profit = 350000 },
//                        new() { Label = "12:00", Revenue = 1200000, Profit = 500000 },
//                        new() { Label = "14:00", Revenue = 600000, Profit = 250000 },
//                        new() { Label = "16:00", Revenue = 500000, Profit = 200000 },
//                        new() { Label = "18:00", Revenue = 700000, Profit = 350000 }
//                    },
//                    CategoryData = new List<CategoryDataPoint>
//                    {
//                        new() { Name = "Cà phê", Percentage = 60, ColorHex = "#D97724" },
//                        new() { Name = "Trà", Percentage = 20, ColorHex = "#F0A04B" },
//                        new() { Name = "Khác", Percentage = 20, ColorHex = "#E8533F" }
//                    }
//                },

//                // 1: TUẦN NÀY (Mốc thời gian theo thứ)
//                1 => new ReportSummary
//                {
//                    TotalRevenue = "32.150.000đ",
//                    RevenueChange = "↘ -2.4%",
//                    EstimatedProfit = "14.200.000đ",
//                    ProfitChange = "↘ -1.8%",
//                    AverageOrder = "52.000đ",
//                    AverageOrderChange = "↗ +1.1%",
//                    TotalOrders = "618",
//                    OrdersChange = "↘ -12",
//                    ChartData = new List<ChartDataPoint>
//                    {
//                        new() { Label = "Thứ 2", Revenue = 3200000, Profit = 1200000 },
//                        new() { Label = "Thứ 3", Revenue = 4100000, Profit = 1800000 },
//                        new() { Label = "Thứ 4", Revenue = 3800000, Profit = 1500000 },
//                        new() { Label = "Thứ 5", Revenue = 5200000, Profit = 2600000 },
//                        new() { Label = "Thứ 6", Revenue = 6100000, Profit = 3200000 },
//                        new() { Label = "Thứ 7", Revenue = 7200000, Profit = 4000000 },
//                        new() { Label = "CN", Revenue = 2550000, Profit = 1000000 } // Cuối tuần mưa ế :)
//                    },
//                    CategoryData = new List<CategoryDataPoint>
//                    {
//                        new() { Name = "Cà phê", Percentage = 40, ColorHex = "#D97724" },
//                        new() { Name = "Trà trái cây", Percentage = 30, ColorHex = "#F0A04B" },
//                        new() { Name = "Sinh tố", Percentage = 30, ColorHex = "#F5D0A9" }
//                    }
//                },

//                // 3: NĂM NAY (Mốc thời gian theo tháng)
//                3 => new ReportSummary
//                {
//                    TotalRevenue = "1.450.800.000đ",
//                    RevenueChange = "↗ +22.5%",
//                    EstimatedProfit = "580.320.000đ",
//                    ProfitChange = "↗ +25.0%",
//                    AverageOrder = "55.000đ",
//                    AverageOrderChange = "↗ +4.5%",
//                    TotalOrders = "26.378",
//                    OrdersChange = "↗ +4.200",
//                    ChartData = new List<ChartDataPoint>
//                    {
//                        new() { Label = "Thg 1", Revenue = 120000000, Profit = 45000000 },
//                        new() { Label = "Thg 2", Revenue = 95000000, Profit = 38000000 },
//                        new() { Label = "Thg 3", Revenue = 135000000, Profit = 55000000 },
//                        new() { Label = "Thg 4", Revenue = 150000000, Profit = 62000000 },
//                        new() { Label = "Thg 5", Revenue = 142000000, Profit = 58000000 },
//                        new() { Label = "Thg 6", Revenue = 160000000, Profit = 68000000 }
//                    },
//                    CategoryData = new List<CategoryDataPoint>
//                    {
//                        new() { Name = "Đồ uống", Percentage = 75, ColorHex = "#D97724" },
//                        new() { Name = "Đồ ăn vặt", Percentage = 15, ColorHex = "#F0A04B" },
//                        new() { Name = "Hạt rang", Percentage = 10, ColorHex = "#E8533F" }
//                    }
//                },

//                // 2: THÁNG NÀY (Mặc định)
//                _ => new ReportSummary
//                {
//                    TotalRevenue = "128.450.000đ",
//                    RevenueChange = "↗ +5.2%",
//                    EstimatedProfit = "42.180.000đ",
//                    ProfitChange = "↗ +5.2%",
//                    AverageOrder = "58.000đ",
//                    AverageOrderChange = "↘ -2.1%",
//                    TotalOrders = "2.214",
//                    OrdersChange = "↗ +5.2%",
//                    ChartData = new List<ChartDataPoint>
//                    {
//                        new() { Label = "Tuần 1", Revenue = 32000000, Profit = 12000000 },
//                        new() { Label = "Tuần 2", Revenue = 28000000, Profit = 9000000 },
//                        new() { Label = "Tuần 3", Revenue = 35000000, Profit = 13500000 },
//                        new() { Label = "Tuần 4", Revenue = 33450000, Profit = 7680000 }
//                    },
//                    CategoryData = new List<CategoryDataPoint>
//                    {
//                        new() { Name = "Cà phê", Percentage = 45, ColorHex = "#D97724" },
//                        new() { Name = "Trà trái cây", Percentage = 25, ColorHex = "#F0A04B" },
//                        new() { Name = "Đồ đá xay", Percentage = 20, ColorHex = "#E8533F" },
//                        new() { Name = "Bánh ngọt", Percentage = 10, ColorHex = "#F5D0A9" }
//                    }
//                }
//            };
//        }
//    }
//}
