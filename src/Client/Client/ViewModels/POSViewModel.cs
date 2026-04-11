using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class POSViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;
        private readonly BillService _billService;
        private readonly TableService _tableService;

        [ObservableProperty]
        public partial ObservableCollection<Category> Categories { get; private set; }

        [ObservableProperty]
        public partial ObservableCollection<Table> Tables {  get; private set; }

        [ObservableProperty]
        public partial Table SelectedTable { get; set; }
        
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FilteredProducts))]
        public partial Category? SelectedCategory { get; set; }

        private List<Product> Products;
        public List<Product> FilteredProducts => GetFilteredProducts();

        public ObservableCollection<BillItem> SelectedProducts { get; set; }
        public bool HasSelection => CheckHasSelection();

        public bool NoSelection => CheckNoSelection();

        [ObservableProperty]
        public partial bool ShowSuccess { get; set; } = false;

        [ObservableProperty]
        public partial bool ShowFailure { get; set; } = false;

        public int TotalBillPrice => GetTotalBillPrice();

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand SaveBillCommand { get; }
        public IAsyncRelayCommand SavePaidBillCommand { get; }

        public POSViewModel(CategoryService categoryService,
                            ProductService productService,
                            BillService billService,
                            TableService tableService) 
        {
            _categoryService = categoryService;
            _productService = productService;
            _billService = billService;
            _tableService = tableService;

            Categories = [];
            Products = [];
            SelectedProducts = [];
            Tables = [];
            SelectedTable = new();

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            LoadCommand.Execute(null);

            SaveBillCommand = new AsyncRelayCommand(SaveBillAsync);
            SavePaidBillCommand = new AsyncRelayCommand(SavePaidBillAsync);

            SelectedProducts.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (BillItem item in e.NewItems)
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (BillItem item in e.OldItems)
                    {
                        item.PropertyChanged -= Item_PropertyChanged;
                    }
                }
                Refresh();
            };
        }

        private async Task LoadAsync()
        {
            try
            {
                var cats = await _categoryService.GetAllCategories();
                var pros = await _productService.GetAllProducts();
                var tabs = await _tableService.GetAllTables();

                Categories = new ObservableCollection<Category>(cats);
                Tables = new ObservableCollection<Table>(tabs);
                Products = [.. pros];

                Categories.Insert(0, new Category
                {
                    CategoryID = 0,
                    CategoryName = "Tất cả"
                });

                Tables.Insert(0, new Table
                {
                    TableID = -1,
                    TableName = "Mang đi",
                });

                SelectedCategory = Categories.First();
                SelectedTable = Tables.First();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while loading: {ex.StackTrace}");
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BillItem.IsNotRemoved))
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            OnPropertyChanged(nameof(TotalBillPrice));
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(NoSelection));
        }

        private void CleanSelectedProducts()
        {
            var toRemove = SelectedProducts.Where(x => !x.IsNotRemoved).ToList();

            foreach (var item in toRemove)
            {
                SelectedProducts.Remove(item);
            }
        }

        [RelayCommand]
        private void SelectProduct(Product product)
        {
            CleanSelectedProducts();
            foreach (var item in SelectedProducts)
            {
                if (product.Name == item.Detail!.Name)
                {
                    item.Count++;
                    OnPropertyChanged(nameof(TotalBillPrice));
                    return;
                }
            }
            SelectedProducts.Add(new BillItem(product, 1));

        }

        [RelayCommand]
        private void ClearOrder()
        {
            SelectedProducts.Clear();
        }

        private async Task SaveBillAsync()
        {
            bool res = await _billService.SaveNewBill([.. SelectedProducts], SelectedTable, false);
            if (res)
            {
                ShowSuccess = true;
            }
            else
            {
                ShowFailure = true;
            }
        }

        private async Task SavePaidBillAsync()
        {
            bool res = await _billService.SaveNewBill([.. SelectedProducts], SelectedTable, true);
            if (res)
            {
                ShowSuccess = true;
            }
            else
            {
                ShowFailure = true;
            }
        }

        private int GetTotalBillPrice()
        {
            int total = 0;
            foreach(var item in SelectedProducts)
            {
                if (item.IsNotRemoved)
                {
                    total += item.TotalPrice;
                }
            }
            return total;
        }

        private bool CheckHasSelection()
        {
            foreach (var item in SelectedProducts)
            {
                if (item.IsNotRemoved)
                    return true;
            }
            return false;
        }

        private bool CheckNoSelection()
        {
            foreach(var item in SelectedProducts)
            {
                if (item.IsNotRemoved)
                    return false;
            }
            return true;
        }

        private List<Product> GetFilteredProducts()
        {
            int? catId = SelectedCategory?.CategoryID;
            if (catId == null || catId == 0)
            {
                return Products;
            }

            return [.. Products.Where(p => p.CategoryID == catId)];
        }
    }
}
