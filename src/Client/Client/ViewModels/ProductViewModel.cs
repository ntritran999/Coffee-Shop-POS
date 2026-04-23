using Client.Models;
using Client.Services;
using Client.Views.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Product> PagedProducts { get; } = new();
        public ObservableCollection<string> SelectedProductImages { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelected))]
        [NotifyPropertyChangedFor(nameof(NoSelected))]
        private Product? _selectedProduct;

        partial void OnSelectedProductChanged(Product? value)
        {
            SelectedProductImages.Clear();

            if (value == null)
            {
                return;
            }

            foreach (var image in SplitImageLinks(value.Image))
            {
                SelectedProductImages.Add(image);
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnPropertyChanged(nameof(CanGoPreviousPage));
                    OnPropertyChanged(nameof(CanGoNextPage));
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    OnPropertyChanged(nameof(CanGoPreviousPage));
                    OnPropertyChanged(nameof(CanGoNextPage));
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public bool HasSelected => SelectedProduct != null;
        public bool NoSelected => SelectedProduct == null;

        public bool CanGoPreviousPage => CurrentPage > 1;
        public bool CanGoNextPage => CurrentPage < TotalPages;
        public string PageInfo => $"Trang {CurrentPage}/{TotalPages}";

        public int PageSize { get; set; } = 8;

        private string _searchKeyword = string.Empty;
        private int? _minPrice;
        private int? _maxPrice;
        private int _selectedCategoryId;
        private string _sortOption = "Mặc định";

        private readonly ProductService _productService;

        public ProductViewModel(ProductService productService)
        {
            _productService = productService;
            _ = _loadProducts();
        }

        public ProductViewModel() : this(App.Services?.GetService<ProductService>() ?? new ProductService())
        {
        }

        public async Task _loadProducts()
        {
            try
            {
                var data = await _productService.GetAllProducts();
                Products.Clear();
                foreach (var item in data)
                {
                    Products.Add(item);
                }

                SelectedProduct = null;
                CurrentPage = 1;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Không thể tải danh sách sản phẩm. {ex.Message}");
            }
        }

        public void SetCategoryFilter(int categoryId)
        {
            _selectedCategoryId = categoryId;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword?.Trim() ?? string.Empty;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetPriceRange(int? minPrice, int? maxPrice)
        {
            _minPrice = minPrice;
            _maxPrice = maxPrice;
            CurrentPage = 1;
            ApplyFilters();
        }

        public void SetSortOption(string? option)
        {
            _sortOption = string.IsNullOrWhiteSpace(option) ? "Mặc định" : option.Trim();
            CurrentPage = 1;
            ApplyFilters();
        }

        public void GoToPreviousPage()
        {
            if (!CanGoPreviousPage)
            {
                return;
            }

            CurrentPage--;
            ApplyFilters();
        }

        public void GoToNextPage()
        {
            if (!CanGoNextPage)
            {
                return;
            }

            CurrentPage++;
            ApplyFilters();
        }

        public void ApplyFilters()
        {
            IEnumerable<Product> query = Products;

            if (_selectedCategoryId > 0)
            {
                query = query.Where(p => p.CategoryID == _selectedCategoryId);
            }

            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                query = query.Where(p =>
                    !string.IsNullOrWhiteSpace(p.Name)
                    && p.Name.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            if (_minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= _minPrice.Value);
            }

            if (_maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= _maxPrice.Value);
            }

            query = _sortOption switch
            {
                "Tên (A-Z)" => query.OrderBy(p => p.Name),
                "Tên (Z-A)" => query.OrderByDescending(p => p.Name),
                "Giá (thấp đến cao)" => query.OrderBy(p => p.Price),
                "Giá (cao đến thấp)" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.ProductID)
            };

            var filtered = query.ToList();
            TotalPages = Math.Max(1, (int)Math.Ceiling(filtered.Count / (double)PageSize));

            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            var pageItems = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedProducts.Clear();
            foreach (var item in pageItems)
            {
                PagedProducts.Add(item);
            }

            if (SelectedProduct != null && PagedProducts.All(p => p.ProductID != SelectedProduct.ProductID))
            {
                SelectedProduct = null;
            }
        }

        public async Task AddProduct()
        {
            var nextProductId = Products.Count == 0 ? 1 : Products.Max(p => p.ProductID) + 1;

            var newProduct = new Product
            {
                ProductID = nextProductId,
                Name = "Sản phẩm mới",
                Price = 0,
                Unit = 1,
                CategoryID = 1,
                Image = ""
            };

            var dlg = new ProductForm();
            await dlg.SetProduct(newProduct, isEdit: false);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    var editedProduct = dlg.Product;
                    if (editedProduct == null)
                    {
                        return;
                    }

                    var createdProduct = await _productService.AddProduct(editedProduct);
                    if (createdProduct.ProductID <= 0)
                    {
                        createdProduct.ProductID = nextProductId;
                    }

                    Products.Add(createdProduct);
                    ApplyFilters();
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi tạo sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        public async Task EditProduct()
        {
            if (SelectedProduct == null)
            {
                return;
            }

            var dlg = new ProductForm();
            await dlg.SetProduct(SelectedProduct, isEdit: true);
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    var updatedProduct = dlg.Product;
                    if (updatedProduct == null)
                    {
                        return;
                    }

                    var success = await _productService.UpdateProduct(updatedProduct);
                    if (!success)
                    {
                        await ShowErrorAsync("Cập nhật sản phẩm thất bại.");
                        return;
                    }

                    var existingIndex = Products.ToList().FindIndex(p => p.ProductID == updatedProduct.ProductID);
                    if (existingIndex >= 0)
                    {
                        Products[existingIndex] = updatedProduct;
                        SelectedProduct = updatedProduct;
                        ApplyFilters();
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi cập nhật sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        public async Task DeleteProduct()
        {
            if (SelectedProduct == null)
            {
                return;
            }

            var dlg = new ConfirmForm();
            dlg.SetMessage($"Bạn có chắc muốn xóa sản phẩm '{SelectedProduct.Name}' không?");
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    var productToDelete = SelectedProduct;
                    var success = await _productService.DeleteProduct(productToDelete.ProductID);
                    if (success)
                    {
                        Products.Remove(productToDelete);
                        SelectedProduct = null;
                        ApplyFilters();
                    }
                    else
                    {
                        await ShowErrorAsync("Xóa sản phẩm thất bại.");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Lỗi khi xóa sản phẩm qua GraphQL: {ex.Message}");
                }
            }
        }

        public async Task<(int successCount, int failedCount, IReadOnlyList<string> errors)> ImportProductsFromFile(
            string filePath,
            IReadOnlyCollection<Category> categories)
        {
            var rows = LoadRowsFromDataSource(filePath);
            var successCount = 0;
            var failedCount = 0;
            var errors = new List<string>();

            foreach (var row in rows)
            {
                try
                {
                    var product = ParseRowToProduct(row, categories);
                    var created = await _productService.AddProduct(product);
                    Products.Add(created);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    errors.Add(ex.Message);
                }
            }

            ApplyFilters();
            return (successCount, failedCount, errors);
        }

        private static IReadOnlyList<DataRow> LoadRowsFromDataSource(string filePath)
        {
            var table = LoadDataTable(filePath);
            return table.Rows.Cast<DataRow>().ToList();
        }

        private static DataTable LoadDataTable(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension == ".csv")
            {
                return LoadCsvDataTable(filePath);
            }

            var connectionString = extension switch
            {
                ".xlsx" => $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1';",
                ".xls" => $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=YES;IMEX=1';",
                ".accdb" or ".mdb" => $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;",
                _ => throw new InvalidOperationException("Định dạng file không được hỗ trợ. Chỉ hỗ trợ CSV (.csv), Excel (.xls, .xlsx) hoặc Access (.mdb, .accdb).")
            };

            using var connection = new OleDbConnection(connectionString);
            connection.Open();

            var tableName = GetSourceTableName(connection, extension);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new InvalidOperationException("Không tìm thấy sheet hoặc table hợp lệ để import.");
            }

            using var adapter = new OleDbDataAdapter($"SELECT * FROM [{tableName}]", connection);
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        private static string? GetSourceTableName(OleDbConnection connection, string extension)
        {
            var schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (schema == null)
            {
                return null;
            }

            var isAccess = extension is ".accdb" or ".mdb";
            foreach (DataRow row in schema.Rows)
            {
                var tableName = row["TABLE_NAME"]?.ToString();
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    continue;
                }

                if (isAccess)
                {
                    var tableType = row["TABLE_TYPE"]?.ToString();
                    if (!string.Equals(tableType, "TABLE", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (tableName.StartsWith("MSys", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!tableName.EndsWith("$", StringComparison.Ordinal) && !tableName.EndsWith("$'", StringComparison.Ordinal))
                    {
                        continue;
                    }
                }

                return tableName;
            }

            return null;
        }

        private static DataTable LoadCsvDataTable(string filePath)
        {
            var lines = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (lines.Count == 0)
            {
                throw new InvalidOperationException("File CSV không có dữ liệu.");
            }

            var firstRow = ParseCsvLine(lines[0]);
            var hasHeader = LooksLikeHeader(firstRow);
            var table = new DataTable();

            if (hasHeader)
            {
                for (var i = 0; i < firstRow.Count; i++)
                {
                    var header = firstRow[i]?.Trim();
                    if (string.IsNullOrWhiteSpace(header))
                    {
                        header = $"Column{i + 1}";
                    }

                    header = EnsureUniqueColumnName(table, header);
                    table.Columns.Add(header);
                }
            }
            else
            {
                for (var i = 0; i < firstRow.Count; i++)
                {
                    table.Columns.Add($"Column{i + 1}");
                }

                AddCsvRow(table, firstRow);
            }

            var startIndex = hasHeader ? 1 : 1;
            for (var i = startIndex; i < lines.Count; i++)
            {
                var values = ParseCsvLine(lines[i]);
                AddCsvRow(table, values);
            }

            return table;
        }

        private static void AddCsvRow(DataTable table, IReadOnlyList<string> values)
        {
            var row = table.NewRow();
            for (var i = 0; i < table.Columns.Count; i++)
            {
                row[i] = i < values.Count ? values[i] : string.Empty;
            }

            table.Rows.Add(row);
        }

        private static string EnsureUniqueColumnName(DataTable table, string columnName)
        {
            var candidate = columnName;
            var index = 1;
            while (table.Columns.Contains(candidate))
            {
                candidate = $"{columnName}_{index}";
                index++;
            }

            return candidate;
        }

        private static bool LooksLikeHeader(IReadOnlyList<string> values)
        {
            if (values.Count == 0)
            {
                return false;
            }

            var knownHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Name", "ProductName", "Tên sản phẩm",
                "Price", "Giá",
                "Unit", "Đơn vị",
                "Category", "CategoryID", "CategoryId", "CategoryName", "Danh mục",
                "Image", "ImageUrl", "Hình ảnh"
            };

            return values.Any(v => knownHeaders.Contains(v?.Trim() ?? string.Empty));
        }

        private static List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            if (line == null)
            {
                return values;
            }

            var current = string.Empty;
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current += '"';
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    values.Add(current.Trim());
                    current = string.Empty;
                    continue;
                }

                current += c;
            }

            values.Add(current.Trim());
            return values;
        }

        private static Product ParseRowToProduct(DataRow row, IReadOnlyCollection<Category> categories)
        {
            var name = GetValue(row, "Name", "ProductName", "Tên sản phẩm") ?? GetValueByIndex(row, 0);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Một dòng dữ liệu không có tên sản phẩm.");
            }

            var priceRaw = GetValue(row, "Price", "Giá") ?? GetValueByIndex(row, 1);
            var price = ParseInt(priceRaw, "Giá sản phẩm không hợp lệ.");
            if (price < 0)
            {
                throw new InvalidOperationException("Giá sản phẩm phải >= 0.");
            }

            var unitRaw = GetValue(row, "Unit", "Đơn vị") ?? GetValueByIndex(row, 2);
            var unit = ParseIntOrDefault(unitRaw, 1);
            if (unit <= 0)
            {
                unit = 1;
            }

            var categoryRaw = GetValue(row, "CategoryID", "CategoryId", "Category", "CategoryName", "Danh mục") ?? GetValueByIndex(row, 3);
            var categoryId = ResolveCategoryId(categoryRaw, categories);

            var image = GetValue(row, "Image", "ImageUrl", "Hình ảnh") ?? GetValueByIndex(row, 4) ?? string.Empty;

            return new Product
            {
                Name = name.Trim(),
                Price = price,
                Unit = unit,
                CategoryID = categoryId,
                Image = image.Trim()
            };
        }

        private static int ResolveCategoryId(string? value, IReadOnlyCollection<Category> categories)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Thiếu thông tin loại sản phẩm (Category).");
            }

            if (int.TryParse(value, out var categoryId) && categoryId > 0)
            {
                return categoryId;
            }

            var byName = categories.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.CategoryName)
                && c.CategoryName.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));

            if (byName == null || byName.CategoryID <= 0)
            {
                throw new InvalidOperationException($"Không tìm thấy loại sản phẩm '{value}'.");
            }

            return byName.CategoryID;
        }

        private static int ParseInt(string? value, string errorMessage)
        {
            if (!int.TryParse(value, out var result))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return result;
        }

        private static int ParseIntOrDefault(string? value, int defaultValue)
        {
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        private static string? GetValue(DataRow row, params string[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (!row.Table.Columns.Contains(candidate))
                {
                    continue;
                }

                var value = row[candidate]?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private static string? GetValueByIndex(DataRow row, int index)
        {
            if (index < 0 || index >= row.Table.Columns.Count)
            {
                return null;
            }

            var value = row[index]?.ToString();
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static IReadOnlyList<string> SplitImageLinks(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return [];
            }

            return raw
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(link => link.Trim())
                .Where(link => !string.IsNullOrWhiteSpace(link))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static async Task ShowErrorAsync(string message)
        {
            var errorDialog = new ContentDialog
            {
                XamlRoot = App.ActiveWindow!.Content.XamlRoot,
                Title = "Thông báo lỗi",
                Content = message,
                PrimaryButtonText = "Đóng"
            };

            await errorDialog.ShowAsync();
        }
    }
}