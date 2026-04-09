using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection; 

namespace Client.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel ViewModel { get; }

        public AccountPage()
        {
            InitializeComponent();

            ViewModel = App.Services.GetService<AccountViewModel>();
        }

        public static SolidColorBrush GetRoleBadgeBackground(bool isAdmin)
        {
            return isAdmin
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 243, 224))  // #FFF3E0
                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 232, 245, 233)); // #E8F5E9
        }

        public static SolidColorBrush GetRoleBadgeBorder(bool isAdmin)
        {
            return isAdmin
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 217, 119, 36))   // #D97724
                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80));   // #4CAF50
        }

        public static SolidColorBrush GetRoleForeground(bool isAdmin)
        {
            return isAdmin
                ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 217, 119, 36))   // #D97724
                : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 76, 175, 80));   // #4CAF50
        }

        public static string GetRoleIcon(bool isAdmin)
        {
            return isAdmin ? "\uE7EF" : "\uE77B";  // Shield vs Person
        }
    }
}