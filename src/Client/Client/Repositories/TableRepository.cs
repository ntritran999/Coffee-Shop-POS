using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public class TableRepository : ITableRepository
    {
        public Task<Table> Add(Table item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Table>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Table?> GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;

            int id = int.Parse(itemId);
            if (id == 1) return new() { TableID = 1, TableName = "Bàn 01", Status = 0 };
            else if (id == 2) return new() { TableID = 2, TableName = "Bàn 02", Status = 1 };
            else if (id == 3) return new() { TableID = 3, TableName = "Bàn 03", Status = 2 };
            else if (id == 4) return new() { TableID = 4, TableName = "Bàn 04", Status = 0 };
            else return null;
        }

        public Task<IEnumerable<Table>> GetByStatus(int status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Table item)
        {
            throw new NotImplementedException();
        }
    }
}
