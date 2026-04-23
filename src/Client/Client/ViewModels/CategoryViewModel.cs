using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class CategoryViewModel : ObservableObject
    {
        public ObservableCollection<Category> Categories { get; private set; } = new();

        [ObservableProperty]
        private Category? _selectedCategory;

        private readonly CategoryService _categoryService;

        public CategoryViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Categories.Add(new Category { CategoryID = 0, CategoryName = "Tất cả" });
            SelectedCategory = Categories.FirstOrDefault();
            _ = LoadCategories();
        }

        public CategoryViewModel() : this(App.Services?.GetService<CategoryService>() ?? new CategoryService())
        {
        }

        public async Task LoadCategories()
        {
            try
            {
                var data = await _categoryService.GetAllCategories();
                Categories.Clear();
                Categories.Add(new Category
                {
                    CategoryID = 0,
                    CategoryName = "Tất cả"
                });

                foreach (var category in data)
                {
                    Categories.Add(category);
                }

                SelectedCategory = Categories.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
                if (Categories.Count == 0)
                {
                    Categories.Add(new Category { CategoryID = 0, CategoryName = "Tất cả" });
                    SelectedCategory = Categories.FirstOrDefault();
                }
            }
        }

        public async Task<Category> AddCategory(string categoryName)
        {
            var normalizedName = categoryName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new InvalidOperationException("Tên loại sản phẩm không được để trống.");
            }

            var existed = Categories.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.CategoryName)
                && c.CategoryName.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));

            if (existed != null)
            {
                return existed;
            }

            var created = await _categoryService.AddCategory(new Category
            {
                CategoryName = normalizedName
            });

            Categories.Add(created);
            return created;
        }
    }
}