using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GrapQL.Models;

namespace GrapQL.Services
{
    // Lightweight GraphQL service using HttpClient. Replace endpoint with your GraphQL server.
    public class GraphQLService : IGraphQLService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public GraphQLService(string endpoint)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _httpClient = new HttpClient();
        }

        public async Task<IReadOnlyList<Bill>> GetBillsAsync()
        {
            var query = @"query { bills { id dateCheckIn dateCheckOut tableID status totalAmount discount } }";
            var payload = new { query };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _httpClient.PostAsync(_endpoint, content);
            resp.EnsureSuccessStatusCode();

            var respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respJson);

            if (!doc.RootElement.TryGetProperty("data", out var data))
                return Array.Empty<Bill>();

            if (!data.TryGetProperty("bills", out var billsElem))
                return Array.Empty<Bill>();

            var list = new List<Bill>();
            foreach (var item in billsElem.EnumerateArray())
            {
                var bill = new Bill
                {
                    ID = item.GetProperty("id").GetString(),
                    DateCheckIn = item.TryGetProperty("dateCheckIn", out var dci) && dci.ValueKind == JsonValueKind.String ? DateTime.Parse(dci.GetString()) : (DateTime?)null,
                    DateCheckOut = item.TryGetProperty("dateCheckOut", out var dco) && dco.ValueKind == JsonValueKind.String ? DateTime.Parse(dco.GetString()) : (DateTime?)null,
                    TableID = item.TryGetProperty("tableID", out var tid) ? tid.GetString() : null,
                    Status = item.TryGetProperty("status", out var st) ? st.GetString() : null,
                    TotalAmount = item.TryGetProperty("totalAmount", out var ta) && ta.TryGetDecimal(out var decTa) ? decTa : 0m,
                    Discount = item.TryGetProperty("discount", out var di) && di.TryGetDecimal(out var decDi) ? decDi : 0m,
                };
                list.Add(bill);
            }

            return list;
        }
    }
}
