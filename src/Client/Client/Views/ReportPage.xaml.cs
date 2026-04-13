using Client.Repositories;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace Client.Views
{
    public sealed partial class ReportPage : Page
    {
        public ReportViewModel ViewModel { get; set; }

        public ReportPage()
        {
            this.InitializeComponent();

            ViewModel = App.Services!.GetRequiredService<ReportViewModel>();

            this.DataContext = ViewModel;
        }
    }
}
