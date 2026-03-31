using Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "http://localhost:5000/graphql";

        public BillRepository()
        {
            _httpClient = new HttpClient();
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
                    Debug.WriteLine("Failed status");
                    return null;
                }

                using var doc = JsonDocument.Parse(respJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
                {
                    Debug.WriteLine($"Errors: {errors}");
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
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Bill> Add(Bill item)
        {
            var mutation = @"
                mutation($data: CreateBillInput) {
                  createBill(data: $data) {
                    BillID
                    DateCheckIn
                    DateCheckOut
                    TableID
                    Status
                    TotalAmount
                    Discount
                  }
                }";

            var variables = new
            {
                data = new
                {
                    TableID = item.TableID,
                    Discount = item.Discount
                }
            };

            var raw = await SendGraphQLFieldRawAsync(mutation, variables, "createBill").ConfigureAwait(false);
            if (raw == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<Bill>(raw, JsonOptions);
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Bill>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Bill>> GetByDate(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public Task<Bill?> GetById(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Bill>> GetByStatus(int status)
        {
            throw new NotImplementedException();
        }

        public Task<Bill?> GetByTable(string tableId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(Bill item)
        {
            if (item.BillID <= 0)
            {
                return false;
            }

            var mutation = @"
                mutation($BillID: Int!, $data: UpdateBillInput) {
                  updateBill(BillID: $BillID, data: $data) {
                    BillID
                  }
                }";

            var variables = new
            {
                BillID = item.BillID,
                data = new
                {
                    TableID = item.TableID,
                    Discount = item.Discount,
                    Status = item.Status,
                    DateCheckOut = item.DateCheckOut.HasValue ? item.DateCheckOut.Value.ToString("o", CultureInfo.InvariantCulture) : null
                }
            };

            var raw = await SendGraphQLFieldRawAsync(mutation, variables, "updateBill").ConfigureAwait(false);
            if (raw == null)
            {
                return false;
            }

            var updated = JsonSerializer.Deserialize<Bill>(raw, JsonOptions);
            return updated?.BillID > 0;
        }
    }
}
