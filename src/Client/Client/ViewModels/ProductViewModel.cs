using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

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

        public ProductViewModel(ProductService? productService = null)
        {
            _productService = productService
                ?? App.Services?.GetService<ProductService>()
                ?? new ProductService();

            var data = _productService.GetAllProducts().Result;
            Products = new ObservableCollection<Product>(data);
            SelectedProduct = null;
        }
    }
}
