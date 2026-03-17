using System;

namespace Client.Repositories;

public interface IAccountRepository : IRepo<Account>
{
    Account Login(string username, string password);
}
