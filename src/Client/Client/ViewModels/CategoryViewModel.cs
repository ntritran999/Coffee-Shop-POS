using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            // Initialize default "Tất cả" category
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
                
                // Clear and reload with fresh data from API
                Categories.Clear();
                
                // Add "Tất cả" category first
                Categories.Add(new Category
                {
                    CategoryID = 0,
                    CategoryName = "Tất cả"
                });

                // Add API data
                foreach (var category in data)
                {
                    Categories.Add(category);
                }

                SelectedCategory = Categories.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
                // Keep the default "Tất cả" category even if loading fails
                if (Categories.Count == 0)
                {
                    Categories.Add(new Category { CategoryID = 0, CategoryName = "Tất cả" });
                    SelectedCategory = Categories.FirstOrDefault();
                }
            }
        }
    }
}