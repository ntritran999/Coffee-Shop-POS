using Client.Helpers;
using Client.Services;
using HarfBuzzSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly string lastScreenSavingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "POSClientSettings", "lastscreen.txt");
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

        public string AccountName = SessionManager.CurrentAccount!.DisplayName;
        public string AccountRole = SessionManager.CurrentAccount!.Role;

        public MainPage()
        {
            InitializeComponent();
            if (!SessionManager.IsAdmin)
            {
                NavAccountPage.Visibility = Visibility.Collapsed;
            }
            if (_pageMap.TryGetValue(LoadLastScreen(), out var entry))
            {
                contentFrame.Navigate(entry.pageType);
                HeaderTitle.Text = entry.title;
            }
            else
            {
                contentFrame.Navigate(typeof(DashboardPage));
            }
        }

        private void navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var tag = args.InvokedItemContainer?.Tag?.ToString();
            if (tag != null && _pageMap.TryGetValue(tag, out var entry))
            {
                if (contentFrame.CurrentSourcePageType != entry.pageType)
                {
                    contentFrame.Navigate(entry.pageType);
                    SaveLastScreen(tag);
                }
                HeaderTitle.Text = entry.title;
            }
        }

        private void NavLogoutItem_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            PerformLogout();
        }

        private string LoadLastScreen()
        {
            if (!LocalSettingsHelper.GetRememberLastScreen() || !File.Exists(lastScreenSavingPath)) return "";

            return File.ReadAllText(lastScreenSavingPath);
        }
        private void SaveLastScreen(string pageTag)
        {
            if (LocalSettingsHelper.GetRememberLastScreen())
            {
                File.WriteAllText(lastScreenSavingPath, pageTag);
            }
        }

        private void PerformLogout()
        {
            try
            {
                // xóa file cấu hình đăng nhập
                string prefsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "login_prefs.txt");
                if (File.Exists(prefsFilePath))
                {
                    File.Delete(prefsFilePath);
                }

                // xóa thông tin phiên làm việc
                SessionManager.CurrentAccount = null;


                if (MainWindow.AppFrame != null)
                {
                    MainWindow.AppFrame.Navigate(typeof(Client.Views.LoginPage));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi: MainWindow.AppFrame bị null, không thể chuyển trang.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đăng xuất: {ex.Message}");
            }
        }
    }
}
