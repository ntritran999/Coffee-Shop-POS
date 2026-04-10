using Client.Models;
using Client.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class TableViewModel : ObservableObject
    {
        private readonly ITableRepository _tableRepository;

        [ObservableProperty]
        public partial ObservableCollection<Table> Tables { get; private set; }

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

        [ObservableProperty]
        public partial string LastUpdatedText { get; set; } = "Chưa cập nhật";

        [ObservableProperty]
        public partial string ErrorMessage { get; set; } = string.Empty;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public int EmptyCount => Tables.Count(x => x.Status == 0);
        public int OccupiedCount => Tables.Count(x => x.Status == 1);
        public int CleaningCount => Tables.Count(x => x.Status == 2);
        public int TotalCount => Tables.Count;

        public IAsyncRelayCommand LoadCommand { get; }

        public TableViewModel(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
            Tables = [];

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            LoadCommand.Execute(null);
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var tables = await _tableRepository.GetAll();
                Tables = new ObservableCollection<Table>(tables.OrderBy(x => x.TableID));
                LastUpdatedText = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                NotifyStatsChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NotifyStatsChanged()
        {
            OnPropertyChanged(nameof(EmptyCount));
            OnPropertyChanged(nameof(OccupiedCount));
            OnPropertyChanged(nameof(CleaningCount));
            OnPropertyChanged(nameof(TotalCount));
        }

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        public string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Trống",
                1 => "Có người",
                2 => "Đang dọn",
                _ => "Không xác định"
            };
        }
    }
}
