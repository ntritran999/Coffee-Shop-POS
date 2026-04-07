using Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "http://localhost:5000/graphql";

        public TableRepository()
        {
            _httpClient = new();
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private async Task<string?> SendGraphQLFieldRawAsync(string query, object? variables, string fieldName)
        {
            try
            {
                var payload = new { query, variables };
                var json = JsonSerializer.Serialize(payload, JsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var resp = await _httpClient.PostAsync(_endpoint, content).ConfigureAwait(false);
                var respJson = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                {
                    return null;
                }

                using var doc = JsonDocument.Parse(respJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
                {
                    return null;
                }

                if (!root.TryGetProperty("data", out var dataElem) || dataElem.ValueKind != JsonValueKind.Object)
                {
                    return null;
                }

                if (!dataElem.TryGetProperty(fieldName, out var fieldElem))
                {
                    return null;
                }

                return fieldElem.GetRawText();
            }
            catch
            {
                return null;
            }
        }

        public async Task<Table> Add(Table item)
        {
            var mutation = @"
                mutation($data: CreateTableInput) {
                  createTable(data: $data) {
                    TableID
                    TableName
                    Status
                  }
                }";

            var variables = new
            {
                data = new
                {
                    TableName = item.TableName,
                    Status = item.Status
                }
            };

            var raw = await SendGraphQLFieldRawAsync(mutation, variables, "createTable").ConfigureAwait(false);
            if (raw == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Table>(raw, JsonOptions);
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Table>> GetAll()
        {
            var query = @"
                query {
                  tables {
                    TableID
                    TableName
                    Status
                  }
                }";

            var raw = await SendGraphQLFieldRawAsync(query, null, "tables").ConfigureAwait(false);
            if (raw == null)
            {
                return [];
            }

            return JsonSerializer.Deserialize<IEnumerable<Table>>(raw, JsonOptions) ?? [];
        }

        public async Task<Table?> GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId) || !int.TryParse(itemId, out int id)) return null;

            var query = @"
                query($TableID: Int!) {
                  table(TableID: $TableID) {
                    TableID
                    TableName
                    Status
                  }
                }";

            var variables = new { TableID = id };

            var raw = await SendGraphQLFieldRawAsync(query, variables, "table").ConfigureAwait(false);
            if (raw == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Table>(raw, JsonOptions);
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

            var mutation = @"
                mutation($TableID: Int!, $data: UpdateTableInput) {
                  updateTable(TableID: $TableID, data: $data) {
                    TableID
                  }
                }";

            var variables = new
            {
                TableID = item.TableID,
                data = new
                {
                    TableName = item.TableName,
                    Status = item.Status
                }
            };

            var raw = await SendGraphQLFieldRawAsync(mutation, variables, "updateTable").ConfigureAwait(false);
            if (raw == null)
            {
                return false;
            }

            var updated = JsonSerializer.Deserialize<Table>(raw, JsonOptions);
            return updated?.TableID > 0;
        }
    }
}
