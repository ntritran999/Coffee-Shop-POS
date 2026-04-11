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
            { "ReportPage",  (typeof(ReportPage),  "Báo cáo Kinh doanh") },
            { "AccountPage", (typeof(AccountPage), "Quản lý Tài khoản") },
            { "SettingPage",  (typeof(SettingsPage), "Cấu hình Hệ thống") },
            { "DashboardPage",  (typeof(DashboardPage),  "Tổng quan hệ thống") },
            { "POSPage", (typeof(POSPage), "Bán hàng") },
            { "OrderPage",  (typeof(OrderListPage), "Danh sách đơn hàng") },
            { "TablePage",  (typeof(TablePage), "Quản lý bàn") },
            { "AIToolPage",  (typeof(AIToolPage), "Trợ lý AI") },
            { "ProductPage", (typeof(ViewProduct), "Sản phẩm") }
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
            if (args.InvokedItemContainer is NavigationViewItem item &&
                item.Tag is string tag &&
                _pageMap.TryGetValue(tag, out var entry))
            {
                if (contentFrame.CurrentSourcePageType != entry.pageType)
                {
                    contentFrame.Navigate(entry.pageType);
                }
                HeaderTitle.Text = entry.title;
            }
        }
    }
}
