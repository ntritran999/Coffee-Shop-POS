using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel; // Cần thiết
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client.ViewModels
{
    // 1. Phải có partial. 
    // Nếu BaseViewModel của bạn chưa kế thừa ObservableObject, hãy đổi sang kế thừa ObservableObject
    public partial class CategoryViewModel : ObservableObject
    {
        // 2. Dùng private set để bảo vệ collection
        public ObservableCollection<Category> Categories { get; private set; }

        private readonly CategoryService _categoryService = new CategoryService();

        // 3. Nếu bạn muốn theo dõi danh mục đang được chọn (SelectedIndex="0" trong XAML)
        [ObservableProperty]
        private Category? _selectedCategory;

        public CategoryViewModel()
        {
            // Lấy dữ liệu từ service
            var data = _categoryService.GetAllCategories().Result;

            // Khởi tạo collection
            Categories = new ObservableCollection<Category>(data);

            // Chèn mục "Tất cả" vào đầu danh sách
            Categories.Insert(0, new Category
            {
                CategoryID = 0,
                CategoryName = "Tất cả"
            });

            // Mặc định chọn mục đầu tiên
            SelectedCategory = Categories.FirstOrDefault();
        }
    }
}