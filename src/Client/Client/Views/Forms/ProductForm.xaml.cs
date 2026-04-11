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

namespace Client.Views.Forms
{
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
                    Unit = product.Unit,
                    Image = product.Image,
                    CategoryID = product.CategoryID
                };
            }

            ValidationErrorText.Text = string.Empty;
            ValidationErrorText.Visibility = Visibility.Collapsed;
            DialogTitle.Text = isEdit ? "Sửa sản phẩm" : "Tạo sản phẩm";
            PrimaryButtonText = isEdit ? "Lưu" : "Tạo";
            SecondaryButtonText = "Hủy";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Product ??= new Product();

            Product.Name = NameBox.Text?.Trim() ?? string.Empty;
            Product.Image = ImageBox.Text?.Trim() ?? string.Empty;

            if (!int.TryParse(PriceBox.Text, out var price))
            {
                price = 0;
            }

            Product.Price = price;

            if (!int.TryParse(UnitBox.Text, out var unit) || unit <= 0)
            {
                ShowValidationError("Đơn vị (Unit) phải là số nguyên lớn hơn 0.");
                args.Cancel = true;
                return;
            }

            if (!int.TryParse(CategoryID.Text, out var categoryId) || categoryId <= 0)
            {
                ShowValidationError("Category phải là số nguyên lớn hơn 0.");
                args.Cancel = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Product.Image) ||
                !(Uri.TryCreate(Product.Image, UriKind.Absolute, out _) || Product.Image.StartsWith("/")))
            {
                ShowValidationError("ImageLink không hợp lệ. Vui lòng nhập URL đầy đủ hoặc đường dẫn bắt đầu bằng '/'.");
                args.Cancel = true;
                return;
            }

            Product.Unit = unit;
            Product.CategoryID = categoryId;
            ValidationErrorText.Text = string.Empty;
            ValidationErrorText.Visibility = Visibility.Collapsed;
        }

        private void ShowValidationError(string message)
        {
            ValidationErrorText.Text = message;
            ValidationErrorText.Visibility = Visibility.Visible;
        }
    }
}
