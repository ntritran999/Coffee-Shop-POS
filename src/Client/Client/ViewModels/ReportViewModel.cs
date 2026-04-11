using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

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

        // Summary metrics
        [ObservableProperty] private string _totalRevenue = string.Empty;
        [ObservableProperty] private string _revenueChange = string.Empty;
        [ObservableProperty] private string _estimatedProfit = string.Empty;
        [ObservableProperty] private string _profitChange = string.Empty;
        [ObservableProperty] private string _averageOrder = string.Empty;
        [ObservableProperty] private string _averageOrderChange = string.Empty;
        [ObservableProperty] private string _totalOrders = string.Empty;
        [ObservableProperty] private string _ordersChange = string.Empty;

        // Selected time filter
        [ObservableProperty] private int _selectedTimeFilter = 2; // 0=Ngày, 1=Tuần, 2=Tháng, 3=Năm

        // Chart data
        public ObservableCollection<RevenueChartItem> WeeklyChartData { get; } = new();
        public ObservableCollection<CategoryItem> CategoryBreakdown { get; } = new();

        public ReportViewModel()
        {
            // Khởi tạo service (nếu bạn có dùng Dependency Injection thì truyền qua Constructor)
            _reportService = new ReportService();

            // Tải dữ liệu lần đầu dựa trên giá trị mặc định (Tháng = 2)
            LoadData(SelectedTimeFilter);
        }

        private void LoadData(int filter)
        {
            var data = _reportService.FetchReportMetrics(filter);

            // Gán dữ liệu vào Properties
            TotalRevenue = data.TotalRevenue;
            RevenueChange = data.RevenueChange;
            EstimatedProfit = data.EstimatedProfit;
            ProfitChange = data.ProfitChange;
            AverageOrder = data.AverageOrder;
            AverageOrderChange = data.AverageOrderChange;
            TotalOrders = data.TotalOrders;
            OrdersChange = data.OrdersChange;

            // Xử lý Chart Data
            double maxValue = 7200000;
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
        private void SelectTimeFilter(string filter)
        {
            SelectedTimeFilter = filter switch
            {
                "Ngày" => 0,
                "Tuần" => 1,
                "Tháng" => 2,
                "Năm" => 3,
                _ => 2
            };

            // Tải lại dữ liệu mỗi khi bấm filter
            LoadData(SelectedTimeFilter);
        }
    }
}
