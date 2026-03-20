using Microsoft.UI.Xaml.Controls;
using Client.ViewModels;

namespace Client.Views
{
    public sealed partial class ReportPage : Page
    {
        public ReportViewModel ViewModel { get; } = new ReportViewModel();

        public ReportPage()
        {
            InitializeComponent();
        }
    }
}
