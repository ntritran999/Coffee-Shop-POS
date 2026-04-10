using Client.Models;
using Client.Services;
using Client.Views.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks; // Thêm để dùng ToList hoặc các hàm lọc

namespace Client.ViewModels
{
    // LỖI 1: Phải có 'partial'
    public partial class ProductViewModel : ObservableObject
    {
        // LỖI 3: Để private set hoặc khởi tạo trực tiếp
        public ObservableCollection<Product> Products { get; private set; }

        // LỖI 2: Field không dùng { get; set; }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelected))]
        [NotifyPropertyChangedFor(nameof(NoSelected))] // Cập nhật cả NoSelected khi SelectedProduct đổi
        private Product? _selectedProduct = null;

        public bool HasSelected => SelectedProduct != null;
        public bool NoSelected => SelectedProduct == null;



        private readonly ProductService _productService = new ProductService();

        public ProductViewModel()
        {
            _ = _loadProducts();
        }
            
        public async Task _loadProducts()
        {
            // Lấy dữ liệu và gán vào ObservableCollection
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