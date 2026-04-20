using Client.Models;
using Client.Services;
using Client.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        [ObservableProperty] private string _dashArray = string.Empty;
        [ObservableProperty] private double _dashOffset = 0;
    }

    public partial class ProductLineUI : ObservableObject
    {
        [ObservableProperty] private int _categoryID;
        [ObservableProperty] private string _productName = string.Empty;
        [ObservableProperty] private string _colorHex = string.Empty;
        [ObservableProperty] private string _pointsString = string.Empty;
    }

    public partial class ReportViewModel : ObservableObject
    {
        private readonly ReportService _reportService;
        private readonly ICategoryRepository _categoryRepository;

        // --- THÔNG SỐ TỔNG QUAN ---
        [ObservableProperty] private string _totalRevenue = string.Empty;
        [ObservableProperty] private string _revenueChange = string.Empty;
        [ObservableProperty] private string _estimatedProfit = string.Empty;
        [ObservableProperty] private string _profitChange = string.Empty;
        [ObservableProperty] private string _averageOrder = string.Empty;
        [ObservableProperty] private string _averageOrderChange = string.Empty;
        [ObservableProperty] private string _totalOrders = string.Empty;
        [ObservableProperty] private string _ordersChange = string.Empty;

        // --- BỘ LỌC THỜI GIAN ---
        [ObservableProperty] private int _selectedTimeFilter = 0; 
        [ObservableProperty] private DateTimeOffset? _fromDate;
        [ObservableProperty] private DateTimeOffset? _toDate;
        [ObservableProperty] private string _selectedDateRangeText = string.Empty;

        // --- TRẠNG THÁI TAB ---
        [ObservableProperty] private Microsoft.UI.Xaml.Visibility _revenueChartVisibility = Microsoft.UI.Xaml.Visibility.Visible;
        [ObservableProperty] private Microsoft.UI.Xaml.Visibility _productChartVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        [ObservableProperty] private string _tab1Background = "#D97724";
        [ObservableProperty] private string _tab1Foreground = "White";
        [ObservableProperty] private string _tab2Background = "Transparent";
        [ObservableProperty] private string _tab2Foreground = "#444444";

        // --- BỘ LỌC DANH MỤC ---
        public ObservableCollection<Category> Categories { get; } = new();

        // Sử dụng Property chuẩn thay vì partial void để tránh lỗi Nullable Reference
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    FilterProductLines();
                }
            }
        }

        private System.Collections.Generic.List<ProductLineUI> _allProductLines = new();

        // --- DANH SÁCH DỮ LIỆU UI ---
        public ObservableCollection<RevenueChartItem> WeeklyChartData { get; } = new();
        public ObservableCollection<CategoryItem> CategoryBreakdown { get; } = new();
        public ObservableCollection<ProductLineUI> ProductLines { get; } = new();

        public ReportViewModel(ReportService reportService, ICategoryRepository categoryRepository)
        {
            _reportService = reportService;
            _categoryRepository = categoryRepository;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            // 1. Tải danh mục
            var cats = await _categoryRepository.GetAll();
            Categories.Clear();
            Categories.Add(new Category { CategoryID = 0, CategoryName = "Tất cả danh mục" });
            
            if (cats != null)
            {
                foreach(var c in cats) Categories.Add(c);
            }
            
            // 2. Chọn mặc định & tải dữ liệu
            SelectedCategory = Categories.First();
            await LoadDataAsync(SelectedTimeFilter);
        }

        [RelayCommand]
        private void SwitchTab(string tab)
        {
            if (tab == "Revenue")
            {
                RevenueChartVisibility = Microsoft.UI.Xaml.Visibility.Visible;
                ProductChartVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                Tab1Background = "#D97724"; Tab1Foreground = "White";
                Tab2Background = "Transparent"; Tab2Foreground = "#444444";
            }
            else
            {
                RevenueChartVisibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                ProductChartVisibility = Microsoft.UI.Xaml.Visibility.Visible;
                Tab1Background = "Transparent"; Tab1Foreground = "#444444";
                Tab2Background = "#D97724"; Tab2Foreground = "White";
            }
        }

        [RelayCommand]
        private async Task SelectTimeFilterAsync(string filter)
        {
            SelectedTimeFilter = filter switch
            {
                "Ngày" => 0, "Tuần" => 1, "Tháng" => 2, "Năm" => 3, _ => 0
            };
            
            SelectedDateRangeText = string.Empty;
            FromDate = null;
            ToDate = null;
            await LoadDataAsync(SelectedTimeFilter);
        }

        [RelayCommand]
        private async Task ApplyDateFilterAsync()
        {
            if (FromDate.HasValue && ToDate.HasValue)
            {
                SelectedDateRangeText = $"{FromDate.Value:dd/MM/yyyy} - {ToDate.Value:dd/MM/yyyy}";
                SelectedTimeFilter = -1; // Tắt highlight các nút Ngày/Tuần...
                
                var data = await _reportService.FetchReportMetricsByDateRangeAsync(FromDate.Value.DateTime, ToDate.Value.DateTime);
                ProcessReportData(data);
            }
        }

        private async Task LoadDataAsync(int filter)
        {
            var data = await _reportService.FetchReportMetricsAsync(filter);
            ProcessReportData(data);
        }

        private void ProcessReportData(ReportSummary data)
        {
            if (data == null) return;

            TotalRevenue = data.TotalRevenue; RevenueChange = data.RevenueChange;
            EstimatedProfit = data.EstimatedProfit; ProfitChange = data.ProfitChange;
            AverageOrder = data.AverageOrder; AverageOrderChange = data.AverageOrderChange;
            TotalOrders = data.TotalOrders; OrdersChange = data.OrdersChange;

            // Xử lý biểu đồ cột
            double maxRevenue = data.ChartData.Any() ? data.ChartData.Max(x => x.Revenue) : 0;
            double maxValue = maxRevenue > 0 ? maxRevenue : 1;

            WeeklyChartData.Clear();
            foreach (var item in data.ChartData)
            {
                WeeklyChartData.Add(new RevenueChartItem
                {
                    DayLabel = item.Label, Revenue = item.Revenue, Profit = item.Profit,
                    RevenueBarHeight = (item.Revenue / maxValue) * 220,
                    ProfitBarHeight = (item.Profit / maxValue) * 220,
                });
            }

            // Xử lý biểu đồ tròn
            CategoryBreakdown.Clear();

            CategoryBreakdown.Clear();

            // Công thức WinUI: StrokeDashArray = (Chu vi / Độ dày nét vẽ)
            // Với Ellipse Width=150, StrokeThickness=30 => Bán kính đường giữa = (150-30)/2 = 60
            // Chu vi thực tế = 2 * PI * 60 = 376.99
            // Quy đổi sang đơn vị Dash (Chu vi thực tế / 30) = 4 * PI = 12.56637
            double totalCircumference = 12.5663706;

            // Offset bắt đầu từ góc 12 giờ (trên cùng) thay vì 3 giờ (mặc định của XAML)
            double currentOffset = totalCircumference * 0.25;

            foreach (var item in data.CategoryData)
            {
                double pct = item.Percentage / 100.0;
                double dash = pct * totalCircumference;
                double gap = totalCircumference - dash;

                CategoryBreakdown.Add(new CategoryItem
                {
                    Name = item.Name,
                    Percentage = item.Percentage,
                    ColorHex = item.ColorHex,
                    // Format ra string cho XAML đọc, dùng InvariantCulture để đảm bảo dấu thập phân là dấu chấm '.'
                    DashArray = $"{dash.ToString(System.Globalization.CultureInfo.InvariantCulture)} {gap.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
                    DashOffset = currentOffset
                });

                // Lùi offset lại cho miếng bánh tiếp theo ghép nối vào
                currentOffset -= dash;
            }

            // Xử lý biểu đồ đường
            _allProductLines.Clear();
            if (data.ProductTrends != null && data.ProductTrends.Any())
            {
                int maxQty = data.ProductTrends.SelectMany(p => p.Quantities).DefaultIfEmpty(1).Max();
                maxQty = maxQty > 0 ? maxQty : 1;

                double chartWidth = 500;
                double chartHeight = 220;
                
                foreach (var trend in data.ProductTrends)
                {
                    var points = new System.Text.StringBuilder();
                    int pointCount = trend.Quantities.Count;
                    double stepX = pointCount > 1 ? chartWidth / (pointCount - 1) : chartWidth;

                    for (int i = 0; i < pointCount; i++)
                    {
                        double x = i * stepX;
                        double y = chartHeight - ((double)trend.Quantities[i] / maxQty * chartHeight);
                        points.Append($"{x.ToString(System.Globalization.CultureInfo.InvariantCulture)},{y.ToString(System.Globalization.CultureInfo.InvariantCulture)} ");
                    }

                    _allProductLines.Add(new ProductLineUI
                    {
                        CategoryID = trend.CategoryID,
                        ProductName = trend.ProductName,
                        PointsString = points.ToString().Trim()
                    });
                }
                
                FilterProductLines(); // Đổ dữ liệu ra UI
            }
        }

        private void FilterProductLines()
        {
            ProductLines.Clear();
            if (SelectedCategory == null) return;

            var filtered = SelectedCategory.CategoryID == 0 
                ? _allProductLines 
                : _allProductLines.Where(p => p.CategoryID == SelectedCategory.CategoryID).ToList();

            string[] trendColors = { "#D97724", "#4A90E2", "#E8533F", "#28A745", "#9B59B6", "#F1C40F", "#1ABC9C", "#34495E", "#E67E22", "#E74C3C" };

            for(int i = 0; i < filtered.Count; i++)
            {
                var item = filtered[i];
                item.ColorHex = trendColors[i % trendColors.Length];
                ProductLines.Add(item);
            }
        }
    }
}