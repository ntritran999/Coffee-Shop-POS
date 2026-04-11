using Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiProductRepository : IProductRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IImageUploadClient _imageUploadClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
        };

        public ApiProductRepository(HttpClient httpClient, IImageUploadClient imageUploadClient)
        {
            _httpClient = httpClient;
            _imageUploadClient = imageUploadClient;
        }

        private async Task<JsonElement?> SendAsync(string query, object? variables, string fieldName)
        {
            var request = new GraphQLRequest
            {
                query = query,
                variables = variables ?? new { }
            };

            var response = await _httpClient.PostAsJsonAsync("", request, JsonOptions).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new HttpRequestException($"GraphQL request failed with status {(int)response.StatusCode}: {errorContent}");
            }

            var result = await response.Content
                .ReadFromJsonAsync<GraphQLResponse<Dictionary<string, JsonElement>>>(JsonOptions)
                .ConfigureAwait(false);

            if (result?.errors != null && result.errors.Count > 0)
            {
                throw new Exception(result.errors[0].message);
            }

            if (result?.data == null || !result.data.TryGetValue(fieldName, out var data))
            {
                return null;
            }

            return data;
        }

        private async Task<string?> ResolveImageValueAsync(string? imageValue)
        {
            if (string.IsNullOrWhiteSpace(imageValue))
            {
                return imageValue;
            }

            var normalized = imageValue.Trim();

            if (File.Exists(normalized))
            {
                return await _imageUploadClient.UploadAsync(normalized).ConfigureAwait(false);
            }

            return normalized;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var query = @"query {
                            products {
                                ProductID
                                Name
                                Price
                                Unit
                                CategoryID
                                Image
                            }
                          }";

            var data = await SendAsync(query, null, "products").ConfigureAwait(false);
            if (data == null)
            {
                return [];
            }

            return JsonSerializer.Deserialize<List<Product>>(data.Value.GetRawText(), JsonOptions) ?? [];
        }

        public async Task<Product?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out var productId))
            {
                return null;
            }

            var query = @"query($productId: Int!) {
                            product(ProductID: $productId) {
                                ProductID
                                Name
                                Price
                                Unit
                                CategoryID
                                Image
                            }
                          }";

            var variables = new { productId };
            var data = await SendAsync(query, variables, "product").ConfigureAwait(false);
            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Product>(data.Value.GetRawText(), JsonOptions);
        }

        public async Task<Product> Add(Product item)
        {
            var mutation = @"mutation($data: CreateProductInput!) {
                                createProduct(data: $data) {
                                    ProductID
                                    Name
                                    Price
                                    Unit
                                    CategoryID
                                    Image
                                }
                              }";

            var image = await ResolveImageValueAsync(item.Image).ConfigureAwait(false);

            var variables = new
            {
                data = new
                {
                    Name = item.Name,
                    Price = item.Price,
                    Unit = item.Unit,
                    CategoryID = item.CategoryID,
                    Image = image,
                }
            };

            var data = await SendAsync(mutation, variables, "createProduct").ConfigureAwait(false);
            var created = data == null
                ? null
                : JsonSerializer.Deserialize<Product>(data.Value.GetRawText(), JsonOptions);

            return created ?? throw new InvalidOperationException("Cannot create product.");
        }

        public async Task<bool> Update(Product item)
        {
            if (item.ProductID <= 0)
            {
                return false;
            }

            var mutation = @"mutation($productId: Int!, $data: UpdateProductInput!) {
                                updateProduct(ProductID: $productId, data: $data) {
                                    ProductID
                                }
                              }";

            var image = await ResolveImageValueAsync(item.Image).ConfigureAwait(false);

            var variables = new
            {
                productId = item.ProductID,
                data = new
                {
                    Name = item.Name,
                    Price = item.Price,
                    Unit = item.Unit,
                    CategoryID = item.CategoryID,
                    Image = image,
                }
            };

            var data = await SendAsync(mutation, variables, "updateProduct").ConfigureAwait(false);
            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return false;
            }

            var updated = JsonSerializer.Deserialize<Product>(data.Value.GetRawText(), JsonOptions);
            return updated?.ProductID > 0;
        }

        public async Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out var productId))
            {
                return false;
            }

            var mutation = @"mutation($productId: Int!) {
                                deleteProduct(ProductID: $productId) {
                                    success
                                }
                              }";

            var data = await SendAsync(mutation, new { productId }, "deleteProduct").ConfigureAwait(false);
            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return false;
            }

            if (data.Value.TryGetProperty("success", out var successElement) &&
                (successElement.ValueKind == JsonValueKind.True || successElement.ValueKind == JsonValueKind.False))
            {
                return successElement.GetBoolean();
            }

            return false;
        }

        public async Task<IEnumerable<Product>> GetByCategory(string categoryId)
        {
            var products = await GetAll().ConfigureAwait(false);
            return products.Where(p => p.CategoryID.ToString() == categoryId);
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            var products = await GetAll().ConfigureAwait(false);
            return products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
