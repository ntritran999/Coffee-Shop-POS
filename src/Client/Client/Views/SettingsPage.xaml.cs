using Client.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Client.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        public SettingsPage()
        {
            InitializeComponent();
            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var child in ItemsPerPageBtnPanel.Children)
            {
                if (child is Button btn && int.Parse((string)btn.Content) == ViewModel.ItemsPerPage)
                {
                    btn.Background = new SolidColorBrush(Color.FromArgb(255, 0xD9, 0x77, 0x24));
                    btn.Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void ItemsPerPage_Click(object sender, RoutedEventArgs e)
        {
            var clickedBtn = sender as Button;
            if (clickedBtn == null) return;

            foreach (var child in ItemsPerPageBtnPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.ClearValue(Button.BackgroundProperty);
                    btn.ClearValue(Button.ForegroundProperty);
                }
            }

            clickedBtn.Background = new SolidColorBrush(Color.FromArgb(255, 0xD9, 0x77, 0x24));
            clickedBtn.Foreground = new SolidColorBrush(Colors.White);
        }
    }
}
