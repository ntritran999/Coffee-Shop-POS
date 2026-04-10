using Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiProductRepository : IProductRepository
    {
        private sealed class DeleteResponsePayload
        {
            public bool success { get; set; }
        }

        private readonly HttpClient _httpClient;

        public ApiProductRepository(HttpClient httpClient)
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

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = await SendGraphQLAsync<List<Product>>(
                @"query {
                    products {
                        ProductID
                        Name
                        Price
                        Unit
                        CategoryID
                        Image
                    }
                }",
                null,
                "products");

            return products ?? [];
        }

        public async Task<Product?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out var id) || id <= 0)
            {
                return null;
            }

            return await SendGraphQLAsync<Product>(
                @"query($id: Int!) {
                    product(ProductID: $id) {
                        ProductID
                        Name
                        Price
                        Unit
                        CategoryID
                        Image
                    }
                }",
                new { id },
                "product");
        }

        public async Task<IEnumerable<Product>> GetByCategory(string categoryId)
        {
            if (!int.TryParse(categoryId, out var id) || id <= 0)
            {
                return [];
            }

            var products = await SendGraphQLAsync<List<Product>>(
                @"query($categoryId: Int) {
                    products(CategoryID: $categoryId) {
                        ProductID
                        Name
                        Price
                        Unit
                        CategoryID
                        Image
                    }
                }",
                new { categoryId = id },
                "products");

            return products ?? [];
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            var products = await SendGraphQLAsync<List<Product>>(
                @"query($name: String) {
                    products(Name: $name) {
                        ProductID
                        Name
                        Price
                        Unit
                        CategoryID
                        Image
                    }
                }",
                new { name },
                "products");

            return products ?? [];
        }

        public async Task<Product> Add(Product item)
        {
            var created = await SendGraphQLAsync<Product>(
                @"mutation($data: CreateProductInput!) {
                    createProduct(data: $data) {
                        ProductID
                        Name
                        Price
                        Unit
                        CategoryID
                        Image
                    }
                }",
                new
                {
                    data = new
                    {
                        item.Name,
                        item.Price,
                        item.Unit,
                        item.CategoryID,
                        item.Image
                    }
                },
                "createProduct");

            return created ?? throw new Exception("Failed to create product.");
        }

        public async Task<bool> Update(Product item)
        {
            var updated = await SendGraphQLAsync<Product>(
                @"mutation($id: Int!, $data: UpdateProductInput!) {
                    updateProduct(ProductID: $id, data: $data) {
                        ProductID
                    }
                }",
                new
                {
                    id = item.ProductID,
                    data = new
                    {
                        item.Name,
                        item.Price,
                        item.Unit,
                        item.CategoryID,
                        item.Image
                    }
                },
                "updateProduct");

            return updated != null;
        }

        public async Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out var productId) || productId <= 0)
            {
                return false;
            }

            var deleted = await SendGraphQLAsync<DeleteResponsePayload>(
                @"mutation($id: Int!) {
                    deleteProduct(ProductID: $id) {
                        success
                    }
                }",
                new { id = productId },
                "deleteProduct");

            return deleted?.success == true;
        }
    }
}
