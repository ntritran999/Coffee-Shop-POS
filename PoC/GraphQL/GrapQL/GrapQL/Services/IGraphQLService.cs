using System.Collections.Generic;
using System.Threading.Tasks;
using GrapQL.Models;

namespace GrapQL.Services
{
    public interface IGraphQLService
    {
        Task<IReadOnlyList<Bill>> GetBillsAsync();
    }
}
