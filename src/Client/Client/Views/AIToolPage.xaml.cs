using Client.ViewModels;
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

namespace Client.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AIToolPage : Page
{
    public ChatViewModel ViewModel { get; } = new();
    public AIToolPage()
    {
        InitializeComponent();

        ViewModel.Messages.CollectionChanged += (s, e) =>
        {
            if (ViewModel.Messages.Count > 0)
                ChatList.ScrollIntoView(ViewModel.Messages.Last());
        };
    }

    private void txtInput_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ViewModel.SendMessageCommand.CanExecute(null))
            {
                ViewModel.SendMessageCommand.Execute(null);
            }
        }
    }

    private async void BtnWriteDescription_Click(object sender, RoutedEventArgs e)
    {
        // Tạo TextBox để người dùng nhập tên món
        TextBox inputTextBox = new TextBox
        {
            PlaceholderText = "VD: Cà phê muối, Trà đào cam sả...",
            Width = 350,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        // Tạo hộp thoại
        ContentDialog dialog = new ContentDialog
        {
            Title = "Gợi ý mô tả món ăn",
            Content = inputTextBox,
            PrimaryButtonText = "Tạo mô tả",
            CloseButtonText = "Hủy",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot // Thuộc tính bắt buộc trong WinUI 3
        };

        // Hiển thị Dialog và chờ kết quả
        var result = await dialog.ShowAsync();

        // Nếu người dùng chọn "Tạo mô tả" và TextBox không trống
        if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(inputTextBox.Text))
        {
            await ViewModel.GenerateFoodDescriptionAsync(inputTextBox.Text);
        }
    }


}
