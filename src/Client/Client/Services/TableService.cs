using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class TableService
    {
        private ITableRepository _tableRepository;
        private IBillRepository _billRepository;

        public TableService(ITableRepository tableRepository, IBillRepository billRepository)
        {
            _tableRepository = tableRepository;
            _billRepository = billRepository;
        }

        public async Task<List<Table>> GetAllTables()
        {
            return [.. await _tableRepository.GetAll()];
        }

        public async Task<Table> AddTable(Table table)
        {
            return await _tableRepository.Add(table);
        }

        public async Task<Boolean> EditTable(Table table)
        {
            return await _tableRepository.Update(table);
        }

        public async Task<List<Bill>> GetBills(Table table)
        {
            return [.. await _billRepository.GetByTable(table.TableID.ToString())];
        }
    }
}
