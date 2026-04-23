using Client.Models;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Views
{
    public sealed partial class ViewProduct : Page
    {
        public ProductViewModel ProductView { get; set; }
        public CategoryViewModel CategoryView { get; set; }

        public ViewProduct()
        {
            InitializeComponent();

            ProductView = App.Services?.GetService<ProductViewModel>() ?? new ProductViewModel();
            CategoryView = App.Services?.GetService<CategoryViewModel>() ?? new CategoryViewModel();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            CategoryList.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;
        }

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = CategoryList.SelectedItem as Category;
            ProductView.SetCategoryFilter(selected?.CategoryID ?? 0);
            UpdateCategoryItemForegrounds();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductView.SetSearchKeyword(SearchBox.Text);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (SortComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            ProductView.SetSortOption(selected);
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var nameBox = new TextBox
            {
                PlaceholderText = "Nhập tên loại sản phẩm"
            };

            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Thêm loại sản phẩm",
                Content = nameBox,
                PrimaryButtonText = "Thêm",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            try
            {
                await CategoryView.AddCategory(nameBox.Text);
            }
            catch (Exception ex)
            {
                await ShowInfoAsync("Không thể thêm loại sản phẩm", ex.Message);
            }
        }

        private void ApplyPriceFilter_Click(object sender, RoutedEventArgs e)
        {
            int? minPrice = TryParsePositiveInteger(MinPriceBox.Text);
            int? maxPrice = TryParsePositiveInteger(MaxPriceBox.Text);

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                _ = ShowInfoAsync("Lọc giá không hợp lệ", "Giá từ phải nhỏ hơn hoặc bằng giá đến.");
                return;
            }

            ProductView.SetPriceRange(minPrice, maxPrice);
        }

        private void ClearPriceFilter_Click(object sender, RoutedEventArgs e)
        {
            MinPriceBox.Text = string.Empty;
            MaxPriceBox.Text = string.Empty;
            ProductView.SetPriceRange(null, null);
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            ProductView.GoToPreviousPage();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            ProductView.GoToNextPage();
        }

        public async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.AddProduct();
        }

        public async void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.EditProduct();
        }

        public async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.DeleteProduct();
        }

        private async void ImportProducts_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.ActiveWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".csv");
            picker.FileTypeFilter.Add(".xlsx");
            picker.FileTypeFilter.Add(".xls");
            picker.FileTypeFilter.Add(".accdb");
            picker.FileTypeFilter.Add(".mdb");

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            try
            {
                var result = await ProductView.ImportProductsFromFile(file.Path, CategoryView.Categories.ToList());
                var message = $"Import thành công: {result.successCount} sản phẩm.\nLỗi: {result.failedCount}.";

                if (result.errors.Count > 0)
                {
                    message += $"\n\nMột số lỗi:\n- {string.Join("\n- ", result.errors.Take(5))}";
                }

                await ShowInfoAsync("Kết quả import", message);
            }
            catch (Exception ex)
            {
                await ShowInfoAsync("Import thất bại", ex.Message);
            }
        }

        private void UpdateCategoryItemForegrounds()
        {
            foreach (var item in CategoryList.Items)
            {
                var container = CategoryList.ContainerFromItem(item) as ListViewItem;
                if (container == null) continue;

                var grid = FindVisualChild<Grid>(container);
                if (grid == null) continue;

                var textBlocks = FindVisualChildren<TextBlock>(grid);
                foreach (var tb in textBlocks)
                {
                    tb.Foreground = container.IsSelected
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(Colors.Black);
                }
            }
        }

        private static int? TryParsePositiveInteger(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return int.TryParse(text, out var value) && value >= 0 ? value : null;
        }

        private async System.Threading.Tasks.Task ShowInfoAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = title,
                Content = message,
                PrimaryButtonText = "Đóng"
            };

            await dialog.ShowAsync();
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static List<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            var list = new List<T>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    list.Add(t);

                list.AddRange(FindVisualChildren<T>(child));
            }

            return list;
        }
    }
}
