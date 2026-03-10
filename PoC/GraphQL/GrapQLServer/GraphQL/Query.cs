using System.Collections.Generic;
using GrapQLServer.Data;
using GrapQLServer.Models;

namespace GrapQLServer.GraphQL
{
    public class Query
    {
        private readonly BillRepository _repo;
        public Query(BillRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Bill> GetBills() => _repo.GetBills();
    }
}
