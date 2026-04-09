using Client.Models;
using Client.Repositories;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly HttpClient _httpClient;

        public AuthService(IAccountRepository accountRepository, HttpClient httpClient)
        {
            _accountRepository = accountRepository;
            _httpClient = httpClient;
        }

        public async Task<Account?> Login(string username, string password)
        {
            var (account, token) = await _accountRepository.LoginAsync(username, password);

            if (account != null && !string.IsNullOrEmpty(token))
            {
                // update session 
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return account;
            }

            return null;
        }

        public async Task Register(string username, string rawPassword, string displayName, string role)
        {
            var newAccount = new Account
            {
                Username = username,
                Password = rawPassword, 
                DisplayName = displayName,
                Role = role
            };

            await _accountRepository.AddAccountAsync(newAccount);
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }
    }
}