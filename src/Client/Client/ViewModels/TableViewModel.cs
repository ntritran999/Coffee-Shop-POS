using Client.Models;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.GenAI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class TableViewModel : ObservableObject
    {
        private readonly TableService _tableService;

        public Table SelectedTable { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<Table> Tables { get; private set; }

        [ObservableProperty]
        public partial ObservableCollection<Bill> Bills { get; private set; }

        [ObservableProperty]
        public partial int NumEmpty { get; set; } = 0;
        [ObservableProperty]
        public partial int NumServing { get; set; } = 0;
        [ObservableProperty]
        public partial int NumCleaning { get; set; } = 0;

        [ObservableProperty]
        public partial bool AreBillsLoading { get; set; } = false;

        public IAsyncRelayCommand LoadCommand { get; }
        public TableViewModel(TableService tableService)
        {
            _tableService = tableService;

            SelectedTable = new();
            Tables = [];
            Bills = [];

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            LoadCommand.Execute(null);
        }

        public async Task LoadAsync()
        {
            try
            {
                var tables = await _tableService.GetAllTables();
                
                Tables = new ObservableCollection<Table>(tables);
                RefreshNums();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while loading tables: {ex.StackTrace}");
            }
        }

        private void RefreshNums()
        {
            NumEmpty = Tables.Count(t => t.Status == 0);
            NumServing = Tables.Count(t => t.Status == 1);
            NumCleaning = Tables.Count(t => t.Status == 2);
        }

        public async Task AddTableCommand(Table newTable)
        {
            try
            {
                var table = await _tableService.AddTable(newTable);
                if (table != null)
                {
                    Tables.Add(table);
                }
                else
                {
                    Debug.WriteLine("Table is null. Could not add.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while adding table: {ex.StackTrace}");
            }
        }

        public async Task EditTableCommand(Table editTable)
        {
            try
            {
                var result = await _tableService.EditTable(editTable);
                if (result)
                {
                    int index = Tables.IndexOf(SelectedTable);
                    if (index != -1)
                    {
                        Tables[index] = editTable;
                        RefreshNums();
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Exception while editing table: {ex.StackTrace}");
            }
        }

        public async Task LoadBillsAsync()
        {
            Bills.Clear();
            AreBillsLoading = true;
            try
            {
                var bills = await _tableService.GetBills(SelectedTable);
                Bills = new ObservableCollection<Bill>(bills);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception while viewing bills: {ex.StackTrace}");
            }
            finally
            {
                AreBillsLoading = false;
            }
        }
    }
}
