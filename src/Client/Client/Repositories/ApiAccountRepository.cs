using Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class ApiAccountRepository : IAccountRepository
    {
        private readonly HttpClient _httpClient;

        // use HttpClient 
        public ApiAccountRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(Account? account, string? token)> LoginAsync(string username, string password)
        {
            var request = new GraphQLRequest
            {
                query = @"mutation($username: String!, $password: String!) {
                            login(Username: $username, Password: $password) {
                                token
                                account {
                                    Username
                                    DisplayName
                                    Role
                                }
                            }
                          }",
                variables = new { username, password }
            };

            var response = await _httpClient.PostAsJsonAsync("", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, LoginData>>>();

            if (result?.errors != null && result.errors.Count > 0)
            {
                throw new Exception(result.errors[0].message);
            }

            var loginData = result?.data["login"];
            return (loginData?.account, loginData?.token);
        }

        public async Task AddAccountAsync(Account account)
        {
            var request = new GraphQLRequest
            {
                query = @"mutation($username: String!, $password: String!, $displayName: String!, $role: String!) {
                            createAccount(Username: $username, Password: $password, DisplayName: $displayName, Role: $role) {
                                Username
                            }
                          }",
                variables = new
                {
                    username = account.Username,
                    password = account.Password,
                    displayName = account.DisplayName,
                    role = account.Role
                }
            };

            var response = await _httpClient.PostAsJsonAsync("", request);
            var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<object>>();

            if (result?.errors != null && result.errors.Count > 0)
            {
                throw new Exception(result.errors[0].message); // "Manager required" error
            }
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var request = new GraphQLRequest
            {
                query = @"query {
                            accounts {
                                Username
                                DisplayName
                                Role
                            }
                          }"
            };

            var response = await _httpClient.PostAsJsonAsync("", request);
            var result = await response.Content.ReadFromJsonAsync<GraphQLResponse<Dictionary<string, List<Account>>>>();

            if (result?.errors != null && result.errors.Count > 0)
            {
                throw new Exception(result.errors[0].message);
            }

            return result?.data["accounts"] ?? new List<Account>();
        }
    }
}