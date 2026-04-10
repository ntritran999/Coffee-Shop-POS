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


}
