using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; 

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

        private readonly ProductService _productService = new ProductService();

        public ProductViewModel()
        {
            var data = _productService.GetAllProducts();
            Products = new ObservableCollection<Product>(data);

            SelectedProduct = null;
        }
    }
}