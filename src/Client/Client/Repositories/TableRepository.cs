using Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly HttpClient _httpClient;

        public TableRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Table> Add(Table item)
        {
            var request = new GraphQLRequest
            {
                query = @"
                mutation($data: CreateTableInput!) {
                  createTable(data: $data) {
                    TableID
                    TableName
                    Status
                  }
                }",
                variables = new
                {
                    data = new
                    {
                        TableName = item.TableName,
                        Status = item.Status
                    }
                }
            };

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };
                var response = await _httpClient.PostAsJsonAsync("", request, options);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, Table>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return null;
                }

                return result?.data?["createTable"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Table>> GetAll()
        {
            var request = new GraphQLRequest
            {
                query = @"
                query {
                  tables {
                    TableID
                    TableName
                    Status
                  }
                }"
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, List<Table>>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return [];
                }

                return result?.data?["tables"] ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<Table?> GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || !int.TryParse(itemId, out int id)) return null;

            var request = new GraphQLRequest
            {
                query = @"
                query($TableID: Int!) {
                  table(TableID: $TableID) {
                    TableID
                    TableName
                    Status
                  }
                }",
                variables = new { TableID = id }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, Table>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return null;
                }

                return result?.data?["table"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<IEnumerable<Table>> GetByStatus(int status)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(Table item)
        {
            if (item.TableID <= 0)
            {
                return false;
            }
            
            var request = new GraphQLRequest
            {
                query = @"
                mutation($TableID: Int!, $data: UpdateTableInput!) {
                  updateTable(TableID: $TableID, data: $data) {
                    TableID
                  }
                }",
                variables = new
                {
                    TableID = item.TableID,
                    data = new
                    {
                        TableName = item.TableName,
                        Status = item.Status
                    }
                }
            };

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };
                var response = await _httpClient.PostAsJsonAsync("", request, options);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, Table>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return false;
                }

                var updated = result?.data?["updateTable"];
                return updated?.TableID > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
