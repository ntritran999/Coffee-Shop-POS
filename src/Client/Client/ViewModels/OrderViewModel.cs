using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Client.ViewModels
{
    public partial class OrderViewModel : ObservableObject
    {
        private readonly BillService _billService;
        private List<OrderLine> _allOrders;
        private List<OrderLine> _filteredOrders;

        private bool? IsDateTimeAsc;
        private bool? IsTotalPriceAsc;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EndOrderNum))]
        public partial ObservableCollection<OrderLine> Orders { get; private set; }

        [ObservableProperty]
        public partial string SearchText { get; set; } = "";

        [ObservableProperty]
        public partial int? StatusFilterRaw { get; private set; } = null;

        [ObservableProperty]
        public partial DateTimeOffset? StartDate { get; set; } = null;

        [ObservableProperty]
        public partial DateTimeOffset? EndDate { get; set; } = null;

        [ObservableProperty]
        public partial int TotalOrders { get; private set; } = 0;

        [ObservableProperty]
        public partial int TotalOrdersToday { get; private set; } = 0;

        [ObservableProperty]
        public partial int TotalInProgress { get; private set; } = 0;

        [ObservableProperty]
        public partial int TotalRevenue { get; private set; } = 0;

        [ObservableProperty]
        public partial double FromTotalPrice { get; set; }

        [ObservableProperty]
        public partial double ToTotalPrice { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StartOrderNum))]
        [NotifyPropertyChangedFor(nameof(EndOrderNum))]
        [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        public partial double PageNumDouble { get; set; } = 1;

        public int StartOrderNum => ((int)PageNumDouble - 1) * TotalLines + 1;

        public int EndOrderNum => Math.Min(_filteredOrders.Count, (int)PageNumDouble * TotalLines);

        [ObservableProperty]
        public partial double TotalPages { get; set; } = 1;
        public int TotalLines = 8;

        [ObservableProperty]
        public partial OrderLine SelectedOrderLine { get; set; }

        [ObservableProperty]
        public partial OrderDetail? SelectedOrderDetail { get; set; }

        [ObservableProperty]
        public partial bool IsDetailLoading { get; set; } = false;

        [ObservableProperty]
        public partial bool IsDetailOpened { get; set; } = false;

        public IAsyncRelayCommand LoadCommand { get; }
        public OrderViewModel(BillService billService)
        {
            _billService = billService;

            Orders = [];
            _allOrders = [];
            _filteredOrders = [];

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            LoadCommand.Execute(null);
        }


        public void FilterDateTime()
        {
            if (IsDateTimeAsc == null)
            {
                IsDateTimeAsc = true;
            }
            else
            {
                IsDateTimeAsc = !IsDateTimeAsc;
            }
            ApplyFilter();
        }

        public void FilterTotalPrice()
        {
            if (IsTotalPriceAsc == null)
            {
                IsTotalPriceAsc = true;
            }
            else
            {
                IsTotalPriceAsc = !IsTotalPriceAsc;
            }
            ApplyFilter();
        }

        private async Task LoadAsync()
        {
            try
            {
                var orders = await _billService.GetOrders();

                _allOrders = orders ?? [];

                foreach (var order in _allOrders)
                {
                    TotalRevenue += order.TotalPrice;
                    if (order.StatusRaw == 0)
                    {
                        TotalInProgress++;
                    }
                    if (order.DateCheckIn == DateTime.Today)
                    {
                        TotalOrdersToday++;
                    }
                }
                if (_allOrders.Count > 0)
                {
                    SelectedOrderLine = _allOrders.First();
                }
                else
                {
                    SelectedOrderLine = new();
                }
                ApplyFilter();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnStartDateChanged(DateTimeOffset? value)
        {
            ApplyFilter();
        }

        partial void OnEndDateChanged(DateTimeOffset? value)
        {
            ApplyFilter();
        }

        partial void OnPageNumDoubleChanged(double value)
        {
            if (value < 1)
            {
                PageNumDouble = 1;
                return;
            }
            ApplyPaging();
        }

        [RelayCommand]
        private void FilterAll()
        {
            StatusFilterRaw = null;
            ApplyFilter();
        }

        [RelayCommand]
        private void FilterNew()
        {
            StatusFilterRaw = 0;
            ApplyFilter();
        }

        [RelayCommand]
        private void FilterPaid()
        {
            StatusFilterRaw = 1;
            ApplyFilter();
        }

        [RelayCommand]
        private void FilterCancelled()
        {
            StatusFilterRaw = 2;
            ApplyFilter();
        }

        [RelayCommand]
        private void FilterTotalPriceRange()
        {
            if (FromTotalPrice > 0 && ToTotalPrice > 0 && FromTotalPrice <= ToTotalPrice)
            {
                ApplyFilter();
            }
        }

        [RelayCommand]
        private void ClearAdvancedFilter()
        {
            FromTotalPrice = 0;
            ToTotalPrice = 0;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = (SearchText ?? "").Trim();
            var filtered = _allOrders.AsEnumerable();

            if (StatusFilterRaw is int statusRaw)
            {
                filtered = filtered.Where(o => o.StatusRaw == statusRaw);
            }

            if (StartDate is DateTimeOffset start)
            {
                var startDate = start.Date;
                filtered = filtered.Where(o => o.DateCheckIn.Date >= startDate);
            }

            if (EndDate is DateTimeOffset end)
            {
                var endDate = end.Date;
                filtered = filtered.Where(o => o.DateCheckIn.Date <= endDate);
            }

            if (IsDateTimeAsc is bool dateTimeAsc)
            {
                if (dateTimeAsc)
                {
                    filtered = [.. filtered.OrderBy(o => o.DateCheckIn)];
                }
                else
                {
                    filtered = [.. filtered.OrderByDescending(o => o.DateCheckIn)];
                }
            }

            if (IsTotalPriceAsc is bool totalPriceAsc)
            {
                if (totalPriceAsc)
                {
                    filtered = [.. filtered.OrderBy(o => o.TotalPrice)];
                }
                else
                {
                    filtered = [.. filtered.OrderByDescending(o => o.TotalPrice)];
                }
            }

            if (FromTotalPrice > 0 && ToTotalPrice > 0)
            {
                filtered = filtered.Where(o => o.TotalPrice >= FromTotalPrice && o.TotalPrice <= ToTotalPrice);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(o =>
                    o.BillID.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (o.TableName ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)
                );
            }

            _filteredOrders = [.. filtered];
            Orders = new ObservableCollection<OrderLine>(filtered);
            TotalOrders = Orders.Count;
            TotalPages = Math.Ceiling((double)Orders.Count / TotalLines);
            PageNumDouble = 1;

            NextPageCommand.NotifyCanExecuteChanged();
            PreviousPageCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPreviousPage))]
        private void PreviousPage()
        {
            PageNumDouble--;
        }

        private bool CanPreviousPage()
        {
            return PageNumDouble > 1;
        }

        [RelayCommand(CanExecute = nameof(CanNextPage))]
        private void NextPage()
        {
            PageNumDouble++;
        }

        private bool CanNextPage()
        {
            return PageNumDouble < TotalPages;
        }

        private void ApplyPaging()
        {
            var paging = _filteredOrders.AsEnumerable();
            paging = paging.Skip(((int)PageNumDouble - 1) * TotalLines).Take(TotalLines);
            Orders = new ObservableCollection<OrderLine>(paging);
        }

        public async Task LoadDetailAsync()
        {
            IsDetailOpened = true;
            IsDetailLoading = true;
            try
            {
                SelectedOrderDetail = await _billService.GetOrderDetail(SelectedOrderLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
            finally
            {
                IsDetailLoading = false;
            }
        }

        [RelayCommand]
        private async Task PayOrder()
        {
            IsDetailLoading = true;
            _ = await _billService.PayOrder(SelectedOrderDetail!);
            _ = LoadAsync();
            IsDetailOpened = false;
            IsDetailLoading = false;
        }

        public async Task CancelOrderCommand()
        {
            IsDetailLoading = true;
            _ = await _billService.CancelOrder(SelectedOrderDetail!);
            _ = LoadAsync();
            IsDetailOpened = false;
            IsDetailLoading = false;
        }
    }
}
