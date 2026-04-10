using Client.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace Client.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly Dictionary<string, (Type pageType, string title)> _pageMap = new()
        {
            { "ReportPage",  (typeof(ReportPage), "Bao cao kinh doanh") },
            { "AccountPage", (typeof(AccountPage), "Quan ly tai khoan") },
            { "SettingPage", (typeof(SettingsPage), "Cau hinh he thong") },
            { "DashboardPage", (typeof(DashboardPage), "Tong quan he thong") },
            { "POSPage", (typeof(POSPage), "Ban hang") },
            { "OrderPage", (typeof(OrderListPage), "Danh sach don hang") },
            { "ProductPage", (typeof(ViewProduct), "Quan ly san pham") },
            { "TablePage", (typeof(TablePage), "Quan ly ban") },
            { "AIToolPage", (typeof(ViewAITool), "AI Tools") }
        };

        public MainPage()
        {
            InitializeComponent();
            if (!SessionManager.IsAdmin)
            {
                NavAccountPage.Visibility = Visibility.Collapsed;
            }

            contentFrame.Navigate(typeof(DashboardPage));
        }

        private void navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is not NavigationViewItem item)
            {
                return;
            }

            if (item.Tag is not string tag)
            {
                return;
            }

            if (!_pageMap.TryGetValue(tag, out var entry))
            {
                return;
            }

            if (contentFrame.CurrentSourcePageType != entry.pageType)
            {
                contentFrame.Navigate(entry.pageType);
            }

            HeaderTitle.Text = entry.title;
        }
    }
}
