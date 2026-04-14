using Client.Helpers;
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
    public sealed partial class ConfigServerDialog : ContentDialog
    {
        public string Host => txtHost.Text.Trim();
        public string Port => txtPort.Text.Trim();

        public ConfigServerDialog()
        {
            this.InitializeComponent();
            this.Loaded += ConfigServerDialog_Loaded;
        }

        private void ConfigServerDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Load cài đặt cũ lên giao diện
            txtHost.Text = LocalSettingsHelper.GetServerHost();
            txtPort.Text = LocalSettingsHelper.GetServerPort();
        }

        private async void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            // Gọi thẳng tên biến đã khai báo bên XAML
            btnTest.IsEnabled = false;

            txtStatus.Text = "Đang kiểm tra kết nối...";
            txtStatus.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Gray);

            var result = await Client.Helpers.LocalSettingsHelper.TestConnectionAsync(Host, Port);

            txtStatus.Text = result.message;
            txtStatus.Foreground = result.isSuccess ?
                new SolidColorBrush(Microsoft.UI.Colors.Green) :
                new SolidColorBrush(Microsoft.UI.Colors.Red);

            // Bật lại nút
            btnTest.IsEnabled = true;
        }
    }
}
