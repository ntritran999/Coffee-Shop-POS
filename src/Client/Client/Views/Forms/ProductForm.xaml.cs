using Client.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage.Pickers;

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

        private async void PickImageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.ActiveWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".webp");

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var current = ImageBox.Text?.Trim() ?? string.Empty;
            ImageBox.Text = string.IsNullOrWhiteSpace(current) ? file.Path : $"{current};{file.Path}";

            Product ??= new Product();
            Product.Image = ImageBox.Text;
            ValidationErrorText.Text = string.Empty;
            ValidationErrorText.Visibility = Visibility.Collapsed;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Product ??= new Product();

            Product.Name = NameBox.Text?.Trim() ?? string.Empty;
            Product.Image = ImageBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                ShowValidationError("Tên sản phẩm không được để trống.");
                args.Cancel = true;
                return;
            }

            if (!int.TryParse(PriceBox.Text, out var price) || price < 0)
            {
                ShowValidationError("Giá phải là số nguyên lớn hơn hoặc bằng 0.");
                args.Cancel = true;
                return;
            }

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

            var imageParts = Product.Image
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (imageParts.Length == 0)
            {
                ShowValidationError("Phải nhập ít nhất 1 ảnh.");
                args.Cancel = true;
                return;
            }

            if (imageParts.Length > 3)
            {
                ShowValidationError("Chỉ được tối đa 3 ảnh, phân tách bằng dấu ';'.");
                args.Cancel = true;
                return;
            }

            foreach (var imageInput in imageParts)
            {
                var validImageInput = File.Exists(imageInput)
                    || (Uri.TryCreate(imageInput, UriKind.Absolute, out var uri)
                        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    || imageInput.StartsWith("/");

                if (!validImageInput)
                {
                    ShowValidationError($"Image không hợp lệ: {imageInput}");
                    args.Cancel = true;
                    return;
                }
            }

            Product.Price = price;
            Product.Unit = unit;
            Product.CategoryID = categoryId;
            Product.Image = string.Join(';', imageParts);

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
