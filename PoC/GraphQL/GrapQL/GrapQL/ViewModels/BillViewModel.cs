using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GrapQL.Models;
using GrapQL.Services;

namespace GrapQL.ViewModels
{
    public class BillViewModel : INotifyPropertyChanged
    {
        private readonly IGraphQLService _graphQLService;

        public ObservableCollection<Bill> Bills { get; } = new ObservableCollection<Bill>();

        public ICommand LoadCommand { get; }

        public BillViewModel(IGraphQLService graphQLService)
        {
            _graphQLService = graphQLService ?? throw new ArgumentNullException(nameof(graphQLService));
            LoadCommand = new RelayCommand(async _ => await LoadAsync(), _ => true);
        }

        public async Task LoadAsync()
        {
            var bills = await _graphQLService.GetBillsAsync();
            Bills.Clear();
            foreach (var b in bills)
                Bills.Add(b);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
