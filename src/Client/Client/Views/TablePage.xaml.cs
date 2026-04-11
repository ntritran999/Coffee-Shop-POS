using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Client.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TablePage : Page
{
    public TableViewModel ViewModel { get; set; }
    public TablePage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<TableViewModel>();
    }

    public bool Negate(bool condition) => !condition;

    private async void AddTable_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddTableForm()
        {
            XamlRoot = this.XamlRoot,
            Title = "Thêm bàn mới",
            PrimaryButtonText = "Thêm",
            CloseButtonText = "Huỷ",
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.AddTableCommand(dialog.NewItem);
        }
    }

    private async void EditTable_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn?.DataContext is Table table)
        {
            ViewModel.SelectedTable = table;
            var dialog = new EditTableForm(table)
            {
                XamlRoot = this.XamlRoot,
                Title = "Cập nhật bàn",
                PrimaryButtonText = "Đồng ý",
                CloseButtonText = "Huỷ",
                DefaultButton = ContentDialogButton.Primary
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.EditTableCommand(dialog.EditItem);
            }
        }
    }

    private async void ViewBills_Click(object sender, RoutedEventArgs e)
    {
        ViewBillsSplitView.IsPaneOpen = true;
        var btn = sender as Button;
        if (btn?.DataContext is Table table)
        {
            ViewModel.SelectedTable = table;
            await ViewModel.LoadBillsAsync();
        }
    }
}
