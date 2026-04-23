using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiCategoryRepository : ICategoryRepository
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
        };

        public ApiCategoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async Task<IEnumerable<Category>> GetAll()
        {
            var query = @"query {
                            categories {
                                CategoryID
                                CategoryName
                            }
                          }";

            var data = await SendAsync(query, null, "categories").ConfigureAwait(false);
            if (data == null)
            {
                return [];
            }

            return JsonSerializer.Deserialize<List<Category>>(data.Value.GetRawText(), JsonOptions) ?? [];
        }

        public async Task<Category?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out var categoryId))
            {
                return null;
            }

            var query = @"query($categoryId: Int!) {
                            category(CategoryID: $categoryId) {
                                CategoryID
                                CategoryName
                            }
                          }";

            var variables = new { categoryId };
            var data = await SendAsync(query, variables, "category").ConfigureAwait(false);

            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Category>(data.Value.GetRawText(), JsonOptions);
        }

        public async Task<Category?> GetByName(string categoryName)
        {
            var query = @"query($name: String!) {
                            categoryByName(CategoryName: $name) {
                                CategoryID
                                CategoryName
                            }
                          }";

            var variables = new { name = categoryName };
            var data = await SendAsync(query, variables, "categoryByName").ConfigureAwait(false);

            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Category>(data.Value.GetRawText(), JsonOptions);
        }

        public async Task<Category> Add(Category item)
        {
            var mutation = @"mutation($categoryName: String!) {
                                createCategory(CategoryName: $categoryName) {
                                    CategoryID
                                    CategoryName
                                }
                              }";

            var variables = new
            {
                categoryName = item.CategoryName,
            };

            var data = await SendAsync(mutation, variables, "createCategory").ConfigureAwait(false);
            var created = data == null
                ? null
                : JsonSerializer.Deserialize<Category>(data.Value.GetRawText(), JsonOptions);

            return created ?? throw new InvalidOperationException("Cannot create category.");
        }

        public async Task<bool> Update(Category item)
        {
            if (item.CategoryID <= 0)
            {
                return false;
            }

            var mutation = @"mutation($categoryId: Int!, $data: UpdateCategoryInput!) {
                                updateCategory(CategoryID: $categoryId, data: $data) {
                                    CategoryID
                                }
                              }";

            var variables = new
            {
                categoryId = item.CategoryID,
                data = new
                {
                    CategoryName = item.CategoryName,
                }
            };

            var data = await SendAsync(mutation, variables, "updateCategory").ConfigureAwait(false);
            if (data == null || data.Value.ValueKind == JsonValueKind.Null)
            {
                return false;
            }

            var updated = JsonSerializer.Deserialize<Category>(data.Value.GetRawText(), JsonOptions);
            return updated?.CategoryID > 0;
        }

        public async Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out var categoryId))
            {
                return false;
            }

            var mutation = @"mutation($categoryId: Int!) {
                                deleteCategory(CategoryID: $categoryId) {
                                    success
                                }
                              }";

            var data = await SendAsync(mutation, new { categoryId }, "deleteCategory").ConfigureAwait(false);
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
    }
}
