using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiCategoryRepository : ICategoryRepository
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<T?> SendGraphQLAsync<T>(string query, object? variables, string fieldName)
        {
            var request = new GraphQLRequest
            {
                query = query,
                variables = variables ?? new { }
            };

            var response = await _httpClient.PostAsJsonAsync("", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, T>>>();
            if (result?.errors != null && result.errors.Count > 0)
            {
                throw new Exception(result.errors[0].message);
            }

            if (result?.data != null && result.data.TryGetValue(fieldName, out var value))
            {
                return value;
            }

            return default;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            var categories = await SendGraphQLAsync<List<Category>>(
                @"query {
                    categories {
                        CategoryID
                        CategoryName
                    }
                }",
                null,
                "categories");

            return categories ?? [];
        }

        public async Task<Category?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out var id) || id <= 0)
            {
                return null;
            }

            var categories = await GetAll();
            return categories.FirstOrDefault(x => x.CategoryID == id);
        }

        public async Task<Category?> GetByName(string categoryName)
        {
            var categories = await GetAll();
            return categories.FirstOrDefault(x => string.Equals(x.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public Task<Category> Add(Category item)
        {
            throw new NotSupportedException("Category mutation is not supported in current API scope.");
        }

        public Task<bool> Update(Category item)
        {
            throw new NotSupportedException("Category mutation is not supported in current API scope.");
        }

        public Task<bool> Delete(string id)
        {
            throw new NotSupportedException("Category mutation is not supported in current API scope.");
        }
    }
}
