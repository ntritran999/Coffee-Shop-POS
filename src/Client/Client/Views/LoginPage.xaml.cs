using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {

        public LoginViewModel ViewModel { get; }
        public LoginPage()
        {
            InitializeComponent();
            ViewModel = App.Services.GetService<LoginViewModel>();
        }

        private async void ConfigServer_Click(object sender, RoutedEventArgs e)
        {
            var configServerDialog = new ConfigServerDialog();
            configServerDialog.XamlRoot = this.XamlRoot;
            configServerDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            configServerDialog.PrimaryButtonText = "Lưu cấu hình";
            configServerDialog.CloseButtonText = "Hủy";
            configServerDialog.DefaultButton = ContentDialogButton.Primary;


            var result = await configServerDialog.ShowAsync();
        }
    }
}
