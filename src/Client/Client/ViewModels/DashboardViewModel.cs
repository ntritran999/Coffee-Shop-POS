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
        private readonly IBillRepository _billRepo = new MockBillRepository();
        private readonly BillService _billService = new();

        [ObservableProperty]
        private ISeries[] seriesCollection;

        [ObservableProperty]
        private string[] labels;

        [ObservableProperty]
        private TimeRange selectedRange;

        public DashboardViewModel()
        {
            _ = LoadRevenue(7);
            _ = LoadDashboardData();
        }

        public async Task LoadRevenue(int days)
        {
            var today = DateTime.Today;
            var fromDate = today.AddDays(-days + 1);

            var bills = await _billRepo.GetByDate(fromDate, today);
            var paidBills = bills.Where(b => b.Status == 1);

            var values = new List<double>();
            var labels = new List<string>();

            for (int i = 0; i < days; i++)
            {
                var date = fromDate.AddDays(i);

                var revenue = paidBills
                    .Where(b => b.DateCheckIn.Date == date.Date)
                    .Sum(b => b.TotalAmount);

                values.Add(revenue);
                labels.Add(date.ToString("dd/MM"));
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

        [ObservableProperty]
        private List<TopSellingProductItem> topSellingProducts = new();

        [ObservableProperty]
        private List<Product> lowStockProducts = new();

        [ObservableProperty]
        private List<RecentOrderItem> recentOrders = new();

        public async Task LoadDashboardData()
        {
            TopSellingProducts = await _billService.GetTopSellingProductsAsync();
            RecentOrders = await _billService.GetRecentOrderDetailsAsync();
        }
    }
}
