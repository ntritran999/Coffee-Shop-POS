using Client.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly HttpClient _httpClient;

        public BillRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Bill> Add(Bill item)
        {
            var request = new GraphQLRequest
            {
                query = @"
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
                }",
                variables = new
                {
                    data = new
                    {
                        TableID = item.TableID,
                        Discount = item.Discount
                    }
                }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, Bill>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return null;
                }

                return result?.data?["createBill"];
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

        public async Task<IEnumerable<Bill>> GetAll()
        {
            var request = new GraphQLRequest
            {
                query = @"
                query {
                  bills {
                    BillID
                    DateCheckIn
                    DateCheckOut
                    TableID
                    Status
                    TotalAmount
                    Discount
                  }
                }"
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, List<Bill>>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return [];
                }

                return result?.data?["bills"] ?? [];
            }
            catch (Exception)
            {
                return [];
            }
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

        public async Task<IEnumerable<Bill>> GetByTable(string tableId)
        {
            if (string.IsNullOrEmpty(tableId) || !int.TryParse(tableId, out int id)) return [];

            var request = new GraphQLRequest
            {
                query = @"
                query($TableID: Int) {
                  bills(TableID: $TableID, Status: 0) {
                    BillID
                    DateCheckIn
                    DateCheckOut
                    TableID
                    Status
                    TotalAmount
                    Discount
                  }
                }",
                variables = new { TableID = id }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, List<Bill>>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return [];
                }

                return result?.data?["bills"] ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<bool> Update(Bill item)
        {
            if (item.BillID <= 0)
            {
                return false;
            }

            var request = new GraphQLRequest
            {
                query = @"
                mutation($BillID: Int!, $data: UpdateBillInput) {
                  updateBill(BillID: $BillID, data: $data) {
                    BillID
                  }
                }",
                variables = new
                {
                    BillID = item.BillID,
                    data = new
                    {
                        TableID = item.TableID,
                        Discount = item.Discount,
                        Status = item.Status,
                        DateCheckOut = item.DateCheckOut.HasValue ? item.DateCheckOut.Value.ToString("o", CultureInfo.InvariantCulture) : null
                    }
                }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", request);
                var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, Bill>>>();

                if (result?.errors != null && result.errors.Count > 0)
                {
                    return false;
                }

                var updated = result?.data?["updateBill"];
                return updated?.BillID > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
