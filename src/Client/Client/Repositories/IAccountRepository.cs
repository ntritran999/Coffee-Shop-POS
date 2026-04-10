using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Repositories
{
    public interface IAccountRepository
    {
        // return account and token
        Task<(Account? account, string? token)> LoginAsync(string username, string password);
        Task AddAccountAsync(Account account);
        Task<List<Account>> GetAllAccountsAsync();
    }
}