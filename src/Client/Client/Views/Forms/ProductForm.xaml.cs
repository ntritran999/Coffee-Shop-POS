using Client.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views.Forms
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductForm : ContentDialog
    {
        public Product? Product { get; set; } = null;
        public ProductForm()
        {
            XamlRoot = App.ActiveWindow!.Content.XamlRoot;
            InitializeComponent();
        }
        public void SetProduct(Product? product, bool isEdit)
        {
            if (product != null)
            {
                Product = new Product
                {
                    ProductID = product.ProductID,
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    CategoryID = product.CategoryID
                };

                //NameBox.Text = Product.Name;
                //PriceBox.Text = Product.Price.ToString();
                //ImageBox.Text = Product.Image;
            }

            DialogTitle.Text = isEdit ? "Sửa sản phẩm" : "Tạo sản phẩm";
            this.PrimaryButtonText = isEdit ? "Lưu" : "Tạo";
            this.SecondaryButtonText = "Hủy";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // simple validation
            Product.Name = NameBox.Text ?? string.Empty;
            if (int.TryParse(PriceBox.Text, out var p))
                Product.Price = p;
            else
                Product.Price = 0;
            Product.Image = ImageBox.Text ?? string.Empty;
        }
    }
}
