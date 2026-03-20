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
        // Summary metrics
        [ObservableProperty] private string _totalRevenue = "128.450.000đ";
        [ObservableProperty] private string _revenueChange = "↗ +5.2%";
        [ObservableProperty] private string _estimatedProfit = "42.180.000đ";
        [ObservableProperty] private string _profitChange = "↗ +5.2%";
        [ObservableProperty] private string _averageOrder = "58.000đ";
        [ObservableProperty] private string _averageOrderChange = "↘ -2.1%";
        [ObservableProperty] private string _totalOrders = "2.214";
        [ObservableProperty] private string _ordersChange = "↗ +5.2%";

        // Selected time filter
        [ObservableProperty] private int _selectedTimeFilter = 2; // 0=Ngày, 1=Tuần, 2=Tháng, 3=Năm

        // Chart data
        public ObservableCollection<RevenueChartItem> WeeklyChartData { get; } = new();
        public ObservableCollection<CategoryItem> CategoryBreakdown { get; } = new();

        public ReportViewModel()
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            // Max value for scaling
            double maxValue = 7200000;

            WeeklyChartData.Clear();
            var days = new (string label, double rev, double profit)[]
            {
                ("Thứ 2", 3200000, 2100000),
                ("Thứ 3", 4100000, 2800000),
                ("Thứ 4", 3800000, 2500000),
                ("Thứ 5", 5200000, 3600000),
                ("Thứ 6", 6100000, 4200000),
                ("Thứ 7", 7200000, 5000000),
                ("CN",    6800000, 4800000),
            };

            foreach (var (label, rev, profit) in days)
            {
                WeeklyChartData.Add(new RevenueChartItem
                {
                    DayLabel = label,
                    Revenue = rev,
                    Profit = profit,
                    RevenueBarHeight = (rev / maxValue) * 220,
                    ProfitBarHeight = (profit / maxValue) * 220,
                });
            }

            CategoryBreakdown.Clear();
            CategoryBreakdown.Add(new CategoryItem { Name = "Cà phê", Percentage = 45, ColorHex = "#D97724" });
            CategoryBreakdown.Add(new CategoryItem { Name = "Trà trái cây", Percentage = 25, ColorHex = "#F0A04B" });
            CategoryBreakdown.Add(new CategoryItem { Name = "Đồ đá xay", Percentage = 20, ColorHex = "#E8533F" });
            CategoryBreakdown.Add(new CategoryItem { Name = "Bánh ngọt", Percentage = 10, ColorHex = "#F5D0A9" });
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
            // In real app: reload data from repository
        }
    }
}
