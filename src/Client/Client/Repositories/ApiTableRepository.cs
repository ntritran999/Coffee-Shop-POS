using Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiTableRepository : ITableRepository
    {
        private sealed class DeleteResponsePayload
        {
            public bool success { get; set; }
        }

        private readonly HttpClient _httpClient;

        public ApiTableRepository(HttpClient httpClient)
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

        public async Task<IEnumerable<Table>> GetAll()
        {
            var tables = await SendGraphQLAsync<List<Table>>(
                @"query {
                    tables {
                        TableID
                        TableName
                        Status
                    }
                }",
                null,
                "tables");

            return tables ?? [];
        }

        public async Task<Table?> GetById(string itemId)
        {
            if (!int.TryParse(itemId, out var id) || id <= 0)
            {
                return null;
            }

            return await SendGraphQLAsync<Table>(
                @"query($id: Int!) {
                    table(TableID: $id) {
                        TableID
                        TableName
                        Status
                    }
                }",
                new { id },
                "table");
        }

        public async Task<IEnumerable<Table>> GetByStatus(int status)
        {
            var tables = await SendGraphQLAsync<List<Table>>(
                @"query($status: Int) {
                    tables(Status: $status) {
                        TableID
                        TableName
                        Status
                    }
                }",
                new { status },
                "tables");

            return tables ?? [];
        }

        public async Task<Table> Add(Table item)
        {
            var created = await SendGraphQLAsync<Table>(
                @"mutation($data: CreateTableInput!) {
                    createTable(data: $data) {
                        TableID
                        TableName
                        Status
                    }
                }",
                new
                {
                    data = new
                    {
                        item.TableName,
                        item.Status
                    }
                },
                "createTable");

            return created ?? throw new Exception("Failed to create table.");
        }

        public async Task<bool> Update(Table item)
        {
            var updated = await SendGraphQLAsync<Table>(
                @"mutation($id: Int!, $data: UpdateTableInput!) {
                    updateTable(TableID: $id, data: $data) {
                        TableID
                    }
                }",
                new
                {
                    id = item.TableID,
                    data = new
                    {
                        item.TableName,
                        item.Status
                    }
                },
                "updateTable");

            return updated != null;
        }

        public async Task<bool> Delete(string id)
        {
            if (!int.TryParse(id, out var tableId) || tableId <= 0)
            {
                return false;
            }

            var deleted = await SendGraphQLAsync<DeleteResponsePayload>(
                @"mutation($id: Int!) {
                    deleteTable(TableID: $id) {
                        success
                    }
                }",
                new { id = tableId },
                "deleteTable");

            return deleted?.success == true;
        }
    }
}
