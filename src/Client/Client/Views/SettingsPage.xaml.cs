using Microsoft.UI.Xaml.Controls;
using Client.ViewModels;

namespace Client.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}
