    using Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class BillInfoRepository : IBillInfoRepository
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
        };

        public BillInfoRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BillInfo> Add(BillInfo item)
        {
            if (item.BillID <= 0 || item.ProductID <= 0 || item.Count <= 0)
            {
                return null;
            }

            var request = new GraphQLRequest
            {
                query = @"
                mutation($data: AddBillItemInput!) {
                  addBillItem(data: $data) {
                    BillInfoID
                    BillID
                    ProductID
                    Count
                    Price
                    Note
                  }
                }",
                variables = new
                {
                    data = new
                    {
                        BillID = item.BillID,
                        ProductID = item.ProductID,
                        Count = item.Count,
                        Price = item.Price,
                        Note = item.Note
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

                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, BillInfo>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return null;
                }

                return result?.data?["addBillItem"];
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

        public Task<IEnumerable<BillInfo>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BillInfo>> GetByBillId(string billId)
        {
            if (!int.TryParse(billId, out int id))
            {
                return [];
            }

            var request = new GraphQLRequest
            {
                query = @"
                query($BillID: Int!) {
                  billInfo(BillID: $BillID) {
                    BillInfoID
                    BillID
                    ProductID
                    Count
                    Price
                    Note
                  }
                }",
                variables = new { BillID = id }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request, JsonOptions);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, List<BillInfo>>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return [];
                }

                return result?.data?["billInfo"] ?? [];
            }
            catch (Exception)
            {
                return [];
            }
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
