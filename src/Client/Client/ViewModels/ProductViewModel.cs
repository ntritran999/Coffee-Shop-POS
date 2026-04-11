using Client.Models;
using Client.Services;
using Client.Views.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
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
            var data = await _productService.GetAllProducts();
            Products = new ObservableCollection<Product>(data);
            SelectedProduct = null;
        }

        public async Task AddProduct()
        {
            var newProduct = new Product
            {
                Name = "Sản phẩm mới",
                Price = 0,
                Unit = 0,
                CategoryID = 0,
                Image = ""
            };
            var dlg = new ProductForm();
            dlg.SetProduct(newProduct, isEdit: false);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                newProduct = dlg.Product;
                await _productService.AddProduct(newProduct);
                Products.Add(newProduct);
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
                Product? updatedProduct = dlg.Product;
                await _productService.UpdateProduct(updatedProduct);
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
                await _productService.DeleteProduct(SelectedProduct.ProductID);
                Products.Remove(SelectedProduct);
            }
        }
    }
}