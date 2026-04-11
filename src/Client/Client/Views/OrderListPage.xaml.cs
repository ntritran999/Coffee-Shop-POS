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
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderListPage : Page
{
    public OrderViewModel ViewModel { get; set; }
    public OrderListPage()
    {
        InitializeComponent();
        FromDate.DateFormat = "{day.integer(2)}/{month.integer(2)}/{year.full}";
        ToDate.DateFormat = "{day.integer(2)}/{month.integer(2)}/{year.full}";
        ViewModel = App.Services!.GetRequiredService<OrderViewModel>();
    }

    private void Detail_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn?.DataContext is OrderLine orderLine)
        {
            ViewModel.SelectedOrderLine = orderLine;
            _ = ViewModel.LoadDetailAsync();
        }
    }

    private async void DeleteOrder_ClickAsync(object sender, RoutedEventArgs e)
    {
        string message = $"Bạn có muốn huỷ đơn hàng {ViewModel.SelectedOrderDetail!.BillID}";
        var dialog = new ContentDialog()
        {
            XamlRoot = this.XamlRoot,
            Title = "Xác nhận huỷ đơn",
            Content = message,
            PrimaryButtonText = "Huỷ",
            CloseButtonText = "Đóng",
            DefaultButton = ContentDialogButton.Primary,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            _ = ViewModel.CancelOrderCommand();
        }
    }

    private void DateTime_Tapped(object sender, TappedRoutedEventArgs e)
    {
        ViewModel.FilterDateTime();
    }

    private void TotalPrice_Tapped(object sender, TappedRoutedEventArgs e)
    {
        ViewModel.FilterTotalPrice();
    }
}
