using Client.Models;
using Client.Services;
using Client.Views.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; private set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelected))]
        [NotifyPropertyChangedFor(nameof(NoSelected))]
        private Product? _selectedProduct = null;

        public bool HasSelected => SelectedProduct != null;
        public bool NoSelected => SelectedProduct == null;

        private readonly ProductService _productService;

        public ProductViewModel(ProductService? productService = null)
        {
            _productService = productService
                ?? App.Services?.GetService<ProductService>()
                ?? new ProductService();

            _ = LoadProductsAsync();
        }

        public async Task LoadProductsAsync()
        {
            var data = await _productService.GetAllProducts();
            Products = new ObservableCollection<Product>(data);
            SelectedProduct = null;
        }

        public async Task AddProduct()
        {
            var newProduct = new Product
            {
                Name = "San pham moi",
                Price = 0,
                Unit = 1,
                CategoryID = 1,
                Image = ""
            };

            var dialog = new ProductForm();
            dialog.SetProduct(newProduct, isEdit: false);
            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary || dialog.Product == null)
            {
                return;
            }

            var createdProduct = await _productService.AddProduct(dialog.Product);
            Products.Add(createdProduct);
            SelectedProduct = createdProduct;
        }

        public async Task EditProduct()
        {
            if (SelectedProduct == null)
            {
                return;
            }

            var dialog = new ProductForm();
            dialog.SetProduct(SelectedProduct, isEdit: true);
            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary || dialog.Product == null)
            {
                return;
            }

            var updated = await _productService.UpdateProduct(dialog.Product);
            if (updated)
            {
                await LoadProductsAsync();
            }
        }

        public async Task DeleteProduct()
        {
            if (SelectedProduct == null)
            {
                return;
            }

            var dialog = new ConfirmForm();
            dialog.SetMessage($"Ban co chac muon xoa san pham '{SelectedProduct.Name}' khong?");
            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            var deleted = await _productService.DeleteProduct(SelectedProduct.ProductID);
            if (deleted)
            {
                Products.Remove(SelectedProduct);
                SelectedProduct = null;
            }
        }
    }
}
