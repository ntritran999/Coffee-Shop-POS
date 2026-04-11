using Client.Models;
using Client.Services;
using Client.Views.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; private set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelected))]
        [NotifyPropertyChangedFor(nameof(NoSelected))]
        private Product? _selectedProduct = null;

        public bool HasSelected => SelectedProduct != null;
        public bool NoSelected => SelectedProduct == null;

        private readonly ProductService _productService;

        public ProductViewModel(ProductService productService)
        {
            _productService = productService;
            _ = _loadProducts();
        }

        public ProductViewModel() : this(App.Services?.GetService<ProductService>() ?? new ProductService())
        {
        }

        public async Task _loadProducts()
        {
            try
            {
                var data = await _productService.GetAllProducts();
                Products = new ObservableCollection<Product>(data);
                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Không thể tải danh sách sản phẩm. {ex.Message}");
            }
        }

        public async Task AddProduct()
        {
            Products ??= new ObservableCollection<Product>();
            var nextProductId = Products.Count == 0 ? 1 : Products.Max(p => p.ProductID) + 1;

            var newProduct = new Product
            {
                ProductID = nextProductId,
                Name = "Sản phẩm mới",
                Price = 0,
                Unit = 1,
                CategoryID = 1,
                Image = ""
            };

            var dlg = new ProductForm();
            dlg.SetProduct(newProduct, isEdit: false);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    newProduct = dlg.Product;
                    var createdProduct = await _productService.AddProduct(newProduct);
                    if (createdProduct.ProductID <= 0)
                    {
                        createdProduct.ProductID = nextProductId;
                    }

                    Products.Add(createdProduct);
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi tạo sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        public async Task EditProduct()
        {
            if (SelectedProduct == null) return;
            var dlg = new ProductForm();
            dlg.SetProduct(SelectedProduct, isEdit: true);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    Product? updatedProduct = dlg.Product;
                    if (updatedProduct == null)
                    {
                        return;
                    }

                    var success = await _productService.UpdateProduct(updatedProduct);
                    if (!success)
                    {
                        await ShowErrorAsync("Cập nhật sản phẩm thất bại.");
                        return;
                    }

                    var existingIndex = Products?.ToList().FindIndex(p => p.ProductID == updatedProduct.ProductID) ?? -1;
                    if (existingIndex >= 0)
                    {
                        Products[existingIndex] = updatedProduct;
                        SelectedProduct = updatedProduct;
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi cập nhật sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        public async Task DeleteProduct()
        {
            if (SelectedProduct == null) return;
            var dlg = new ConfirmForm();
            dlg.SetMessage($"Bạn có chắc muốn xóa sản phẩm '{SelectedProduct.Name}' không?");
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    var success = await _productService.DeleteProduct(SelectedProduct.ProductID);
                    if (success)
                    {
                        Products.Remove(SelectedProduct);
                    }
                    else
                    {
                        await ShowErrorAsync("Xóa sản phẩm thất bại.");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi xóa sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        private static async Task ShowErrorAsync(string message)
        {
            var errorDialog = new ContentDialog
            {
                XamlRoot = App.ActiveWindow!.Content.XamlRoot,
                Title = "Thông báo lỗi",
                Content = message,
                PrimaryButtonText = "Đóng"
            };

            await errorDialog.ShowAsync();
        }
    }
}