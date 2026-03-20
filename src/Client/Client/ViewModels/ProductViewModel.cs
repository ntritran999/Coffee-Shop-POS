using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; // Thêm để dùng ToList hoặc các hàm lọc

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
            // Lấy dữ liệu và gán vào ObservableCollection
            var data = _productService.GetAllProducts();
            Products = new ObservableCollection<Product>(data);

            SelectedProduct = null;
        }
    }
}