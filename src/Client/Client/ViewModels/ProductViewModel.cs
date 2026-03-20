using Client.Models;
using Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Client.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public ObservableCollection<Product> Products { get; set; } = new();

        public Product? SelectedProduct { get; set; } = null;
        public bool HasSelected => SelectedProduct != null;
        public bool NoSelected => SelectedProduct == null;
        ProductService _productService = new ProductService();

        public ProductViewModel() 
        {
            Products = new ObservableCollection<Product>(
                _productService.GetAllProducts()
            );

            SelectedProduct = null;
        }

    }
}
