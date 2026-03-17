using System;

namespace Client.Repositories;

public interface ICategoryRepository : IRepo<Category>
{
    Category GetByName(string categoryName);
}
