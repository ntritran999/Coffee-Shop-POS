    using Client.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class BillInfoRepository : IBillInfoRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint = "http://localhost:5000/graphql";
        public BillInfoRepository()
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

        public async Task<BillInfo> Add(BillInfo item)
        {
            if (item.BillID <= 0 || item.ProductID <= 0 || item.Count <= 0)
            {
                return null;
            }

            var mutation = @"
                mutation($data: AddBillItemInput!) {
                  addBillItem(data: $data) {
                    BillInfoID
                    BillID
                    ProductID
                    Count
                    Price
                    Note
                  }
                }";

            var variables = new
            {
                data = new
                {
                    BillID = item.BillID,
                    ProductID = item.ProductID,
                    Count = item.Count,
                    Price = item.Price,
                    Note = item.Note
                }
            };

            var raw = await SendGraphQLFieldRawAsync(mutation, variables, "addBillItem").ConfigureAwait(false);
            if (raw == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<BillInfo>(raw, JsonOptions);
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BillInfo>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BillInfo>> GetByBillId(string billId)
        {
            throw new NotImplementedException();
        }

        public Task<BillInfo?> GetById(string itemId)
        {
            throw new NotImplementedException();
        }

        public Task<BillInfo?> GetItem(string billId, string productId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(BillInfo item)
        {
            throw new NotImplementedException();
        }
    }
}
