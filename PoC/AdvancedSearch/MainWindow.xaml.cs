using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AdvancedSearch.Models;

namespace AdvancedSearch
{
    public sealed partial class MainWindow : Window
    {
        private List<Product> _allProducts;
        public ObservableCollection<Product> FilteredProducts { get; } = new ObservableCollection<Product>();

        public MainWindow()
        {
            this.InitializeComponent();
            LoadDummyData();
            InitializeFilters();
        }

        private void LoadDummyData()
        {
            _allProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Espresso", Category = "Coffee", Price = 2.50, Tags = "hot, strong" },
                new Product { Id = 2, Name = "Latte", Category = "Coffee", Price = 3.50, Tags = "hot, milk" },
                new Product { Id = 3, Name = "Cappuccino", Category = "Coffee", Price = 3.50, Tags = "hot, foam" },
                new Product { Id = 4, Name = "Iced Mocha", Category = "Coffee", Price = 4.00, Tags = "cold, chocolate" },
                new Product { Id = 5, Name = "Green Tea", Category = "Tea", Price = 2.00, Tags = "hot, healthy" },
                new Product { Id = 6, Name = "Black Tea", Category = "Tea", Price = 2.00, Tags = "hot, classic" },
                new Product { Id = 7, Name = "Boba Milk Tea", Category = "Tea", Price = 4.50, Tags = "cold, sweet, tapioca" },
                new Product { Id = 8, Name = "Croissant", Category = "Pastry", Price = 3.00, Tags = "food, butter" },
                new Product { Id = 9, Name = "Chocolate Chip Cookie", Category = "Pastry", Price = 1.50, Tags = "food, sweet" },
                new Product { Id = 10, Name = "AeroPress Maker", Category = "Equipment", Price = 29.99, Tags = "tool, brewing" }
            };
            ExecuteSearch(new SearchCriteria()); // Load all initially
        }

        private void InitializeFilters()
        {
            var categories = _allProducts.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
            categories.Insert(0, "All");
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var criteria = new SearchCriteria
            {
                Keyword = SearchKeywordBox.Text,
                Category = CategoryComboBox.SelectedItem as string
            };
            
            criteria.MinPrice = double.IsNaN(MinPriceBox.Value) ? (double?)null : MinPriceBox.Value;
            criteria.MaxPrice = double.IsNaN(MaxPriceBox.Value) ? (double?)null : MaxPriceBox.Value;

            ExecuteSearch(criteria);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchKeywordBox.Text = string.Empty;
            CategoryComboBox.SelectedIndex = 0;
            MinPriceBox.Value = double.NaN;
            MaxPriceBox.Value = double.NaN;

            ExecuteSearch(new SearchCriteria());
        }

        private void ExecuteSearch(SearchCriteria criteria)
        {
            var query = _allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                var keyword = criteria.Keyword.Trim();
                query = query.Where(p => 
                    p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                    (p.Tags != null && p.Tags.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(criteria.Category) && criteria.Category != "All")
            {
                query = query.Where(p => p.Category == criteria.Category);
            }

            if (criteria.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= criteria.MinPrice.Value);
            }

            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
            }

            var results = query.ToList();
            FilteredProducts.Clear();
            foreach (var item in results)
            {
                FilteredProducts.Add(item);
            }
            
            ResultSummaryText.Text = $"Results: {results.Count} items found";
        }
    }
}
