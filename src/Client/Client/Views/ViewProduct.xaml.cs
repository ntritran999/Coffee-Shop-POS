using Client.Models;
using Client.ViewModels;
using Client.Views.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class ViewProduct : Page
    {
        public ProductViewModel ProductView { get; set; }
        public CategoryViewModel CategoryView { get; set; } = new();

        public ViewProduct()
        {
            InitializeComponent();

            ProductView = App.Services?.GetService<ProductViewModel>() ?? new ProductViewModel();

            this.Loaded += (s, e) =>
            {
                CategoryList.SelectedIndex = 0;
            };
        }

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ===== 1. LOGIC FILTER (giữ nguyên của bạn) =====
            var selected = CategoryList.SelectedItem as Category;

            int categoryId = selected != null ? selected.CategoryID : 0;

            if (categoryId == 0)
            {
                ProductList.ItemsSource = ProductView.Products;
            }
            else
            {
                ProductList.ItemsSource = ProductView.Products
                    .Where(p => p.CategoryID == categoryId);
            }

            // ===== 2. UPDATE UI (thêm vào) =====
            foreach (var item in CategoryList.Items)
            {
                var container = CategoryList.ContainerFromItem(item) as ListViewItem;
                if (container == null) continue;

                var grid = FindVisualChild<Grid>(container);
                if (grid == null) continue;

                var textBlocks = FindVisualChildren<TextBlock>(grid);

                foreach (var tb in textBlocks)
                {
                    tb.Foreground = container.IsSelected
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(Colors.Black);
                }

                // Update badge (nếu có Border)
                var borders = FindVisualChildren<Border>(grid);
                foreach (var border in borders)
                {
                    // chỉ đổi badge (nhỏ nhỏ bên phải)
                    if (border.CornerRadius.TopLeft == 20)
                    {
                        border.Background = container.IsSelected
                            ? new SolidColorBrush(Colors.White)
                            : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 230, 230, 230));
                    }
                }
            }
        }

        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static List<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            var list = new List<T>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                    list.Add(t);

                list.AddRange(FindVisualChildren<T>(child));
            }

            return list;
        }

        public async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.AddProduct();
        }

        public async void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.EditProduct();
        }

        public async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            await ProductView.DeleteProduct();
        }
    }
}
