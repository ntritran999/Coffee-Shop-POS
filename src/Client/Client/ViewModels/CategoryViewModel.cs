using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client.ViewModels
{
    public partial class CategoryViewModel : ObservableObject
    {
        public ObservableCollection<Category> Categories { get; private set; }

        private readonly CategoryService _categoryService;

        [ObservableProperty]
        private Category? _selectedCategory;

        public CategoryViewModel(CategoryService? categoryService = null)
        {
            _categoryService = categoryService
                ?? App.Services?.GetService<CategoryService>()
                ?? new CategoryService();

            var data = _categoryService.GetAllCategories().Result;

            Categories = new ObservableCollection<Category>(data);
            Categories.Insert(0, new Category
            {
                CategoryID = 0,
                CategoryName = "Tất cả"
            });

            SelectedCategory = Categories.FirstOrDefault();
        }
    }
}
