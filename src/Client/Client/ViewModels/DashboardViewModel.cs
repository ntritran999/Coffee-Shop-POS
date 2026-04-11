using Client.Models;
using Client.Repositories;
using Client.Services;
using Client.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public enum TimeRange
    {
        Week,
        Month,
        Quarter
    }
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly BillService _billService;

        [ObservableProperty]
        private ISeries[] seriesCollection;

        [ObservableProperty]
        private string[] labels;

        [ObservableProperty]
        private TimeRange selectedRange = TimeRange.Week;

        [ObservableProperty]
        private List<TopSellingProductItem> topSellingProducts = new();

        [ObservableProperty]
        private List<RecentOrderItem> recentOrders = new();

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private double totalRevenue = 0;

        [ObservableProperty]
        private int totalBills = 0;

        public DashboardViewModel(BillService billService)
        {
            _billService = billService;
            _ = InitializeDashboard();
        }

        private async Task InitializeDashboard()
        {
            IsLoading = true;
            try
            {
                await LoadRevenue(7);
                await LoadDashboardData();
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadRevenue(int days)
        {
            try
            {
                var today = DateTime.Today;
                var fromDate = today.AddDays(-days + 1);

                var bills = await _billService.GetRevenueByDateAsync(fromDate, today);
                var billList = bills.ToList();

                var paidBills = billList.Where(b => b.Status == 1).ToList();

                var values = new List<double>();
                var labels = new List<string>();

                // Calculate total revenue for the period
                double periodRevenue = 0;

                for (int i = 0; i < days; i++)
                {
                    var date = fromDate.AddDays(i);

                    var revenue = paidBills
                        .Where(b => b.DateCheckIn.Date == date.Date)
                        .Sum(b => b.TotalAmount);

                    values.Add(revenue);
                    labels.Add(date.ToString("dd/MM"));
                    periodRevenue += revenue;
                }

                SeriesCollection = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Name = "Doanh thu",
                        Values = values
                    }
                };

                Labels = labels.ToArray();
                TotalRevenue = periodRevenue;
                TotalBills = paidBills.Count;
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
        }

        [RelayCommand]
        public async Task LoadWeek()
        {
            SelectedRange = TimeRange.Week;
            await LoadRevenue(7);
        }

        [RelayCommand]
        public async Task LoadMonth()
        {
            SelectedRange = TimeRange.Month;
            await LoadRevenue(30);
        }

        [RelayCommand]
        public async Task LoadQuarter()
        {
            SelectedRange = TimeRange.Quarter;
            await LoadRevenue(90);
        }

        public async Task LoadDashboardData()
        {
            try
            {
                // Load top 5 selling products
                var topProducts = await _billService.GetTopSellingProductsAsync(top: 5);
                TopSellingProducts = topProducts ?? new();

                // Load recent 3 orders
                var recentOrdersList = await _billService.GetRecentOrderDetailsAsync(top: 3);
                RecentOrders = recentOrdersList ?? new();
            }
            catch (Exception ex)
            {
                // Log error if needed
                TopSellingProducts = new();
                RecentOrders = new();
            }
        }

        [RelayCommand]
        public async Task RefreshDashboard()
        {
            await InitializeDashboard();
        }
    }
}
