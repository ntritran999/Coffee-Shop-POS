using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client.ViewModels
{

    public partial class RevenueChartItem : ObservableObject
    {
        [ObservableProperty] private string _dayLabel = string.Empty;
        [ObservableProperty] private double _revenue;
        [ObservableProperty] private double _profit;
        [ObservableProperty] private double _revenueBarHeight;
        [ObservableProperty] private double _profitBarHeight;
    }

    public partial class CategoryItem : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private int _percentage;
        [ObservableProperty] private string _colorHex = "#D97724";
    }

    public partial class ReportViewModel : ObservableObject
    {
        private readonly ReportService _reportService;

        [ObservableProperty]
        private DateTimeOffset? _fromDate;

        [ObservableProperty]
        private DateTimeOffset? _toDate;

        [ObservableProperty]
        private string _selectedDateRangeText = "Chọn khoảng thời gian";

        // Summary metrics
        [ObservableProperty] private string _totalRevenue = string.Empty;
        [ObservableProperty] private string _revenueChange = string.Empty;
        [ObservableProperty] private string _estimatedProfit = string.Empty;
        [ObservableProperty] private string _profitChange = string.Empty;
        [ObservableProperty] private string _averageOrder = string.Empty;
        [ObservableProperty] private string _averageOrderChange = string.Empty;
        [ObservableProperty] private string _totalOrders = string.Empty;
        [ObservableProperty] private string _ordersChange = string.Empty;

        // Selected time filter (0=Ngày, 1=Tuần, 2=Tháng, 3=Năm) - Mặc định là 0 (Hôm nay)
        [ObservableProperty] private int _selectedTimeFilter = 0;

        // Chart data
        public ObservableCollection<RevenueChartItem> WeeklyChartData { get; } = new();
        public ObservableCollection<CategoryItem> CategoryBreakdown { get; } = new();

        // Tiêm ReportService thông qua DI Container
        public ReportViewModel(ReportService reportService)
        {
            _reportService = reportService;

            // Tải dữ liệu lần đầu (Fire and forget)
            _ = LoadDataAsync(SelectedTimeFilter);
        }

        private async Task LoadDataAsync(int filter)
        {
            // Lấy dữ liệu bất đồng bộ từ Service
            var data = await _reportService.FetchReportMetricsAsync(filter);

            if (data == null) return;

            // Gán dữ liệu vào Properties để UI cập nhật
            TotalRevenue = data.TotalRevenue;
            RevenueChange = data.RevenueChange;
            EstimatedProfit = data.EstimatedProfit;
            ProfitChange = data.ProfitChange;
            AverageOrder = data.AverageOrder;
            AverageOrderChange = data.AverageOrderChange;
            TotalOrders = data.TotalOrders;
            OrdersChange = data.OrdersChange;

            // Xử lý Chart Data (Tính MaxValue động để cột không bị tràn)
            double maxRevenue = data.ChartData.Any() ? data.ChartData.Max(x => x.Revenue) : 0;
            double maxValue = maxRevenue > 0 ? maxRevenue : 1; // Tránh lỗi chia cho 0

            WeeklyChartData.Clear();
            foreach (var item in data.ChartData)
            {
                WeeklyChartData.Add(new RevenueChartItem
                {
                    DayLabel = item.Label,
                    Revenue = item.Revenue,
                    Profit = item.Profit,
                    // Chiều cao tối đa là 220, tỷ lệ tự động co giãn
                    RevenueBarHeight = (item.Revenue / maxValue) * 220,
                    ProfitBarHeight = (item.Profit / maxValue) * 220,
                });
            }

            // Xử lý Category Data
            CategoryBreakdown.Clear();
            foreach (var item in data.CategoryData)
            {
                CategoryBreakdown.Add(new CategoryItem
                {
                    Name = item.Name,
                    Percentage = item.Percentage,
                    ColorHex = item.ColorHex
                });
            }
        }

        [RelayCommand]
        private async Task ApplyDateFilterAsync()
        {
            if (FromDate.HasValue && ToDate.HasValue)
            {
                SelectedDateRangeText = $"{FromDate.Value:dd/MM/yyyy} - {ToDate.Value:dd/MM/yyyy}";
                SelectedTimeFilter = -1; // Reset highlight của các nút Ngày/Tuần/Tháng/Năm

                // Lấy dữ liệu từ Service theo ngày người dùng chọn
                var data = await _reportService.FetchReportMetricsByDateRangeAsync(FromDate.Value.DateTime, ToDate.Value.DateTime);

                if (data != null)
                {
                    // --- Copy lại phần gán UI từ hàm LoadDataAsync cũ xuống đây ---
                    TotalRevenue = data.TotalRevenue;
                    RevenueChange = data.RevenueChange;
                    EstimatedProfit = data.EstimatedProfit;
                    ProfitChange = data.ProfitChange;
                    AverageOrder = data.AverageOrder;
                    AverageOrderChange = data.AverageOrderChange;
                    TotalOrders = data.TotalOrders;
                    OrdersChange = data.OrdersChange;

                    double maxRevenue = data.ChartData.Any() ? data.ChartData.Max(x => x.Revenue) : 0;
                    double maxValue = maxRevenue > 0 ? maxRevenue : 1;

                    WeeklyChartData.Clear();
                    foreach (var item in data.ChartData)
                    {
                        WeeklyChartData.Add(new RevenueChartItem
                        {
                            DayLabel = item.Label,
                            Revenue = item.Revenue,
                            Profit = item.Profit,
                            RevenueBarHeight = (item.Revenue / maxValue) * 220,
                            ProfitBarHeight = (item.Profit / maxValue) * 220,
                        });
                    }

                    CategoryBreakdown.Clear();
                    foreach (var item in data.CategoryData)
                    {
                        CategoryBreakdown.Add(new CategoryItem { Name = item.Name, Percentage = item.Percentage, ColorHex = item.ColorHex });
                    }
                }
            }
        }

        // Dùng RelayCommand với Async để UI không bị treo khi fetch data
        [RelayCommand]
        private async Task SelectTimeFilterAsync(string filter)
        {
            SelectedTimeFilter = filter switch
            {
                "Ngày" => 0,
                "Tuần" => 1,
                "Tháng" => 2,
                "Năm" => 3,
                _ => 0
            };

            SelectedDateRangeText = string.Empty;
            FromDate = null;
            ToDate = null;

            await LoadDataAsync(SelectedTimeFilter);
        }
    }
}
