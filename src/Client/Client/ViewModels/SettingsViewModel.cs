using Client.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.AppLifecycle;
using System;

namespace Client.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        // Display settings
        [ObservableProperty] private int _itemsPerPage = 10;
        [ObservableProperty] private bool _rememberLastScreen = true;

        // Connection settings
        [ObservableProperty] private string _serverHost;
        [ObservableProperty] private string _serverPort;

        // Connection test status
        [ObservableProperty] private string _connectionStatus = string.Empty;
        [ObservableProperty] private bool _isTestingConnection;

        // Last updated label
        [ObservableProperty] private string _lastUpdated = $"Lần cập nhật cuối: {DateTime.Now:dd/MM/yyyy - HH:mm}";

        public SettingsViewModel()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            ServerHost = LocalSettingsHelper.GetServerHost();
            ServerPort = LocalSettingsHelper.GetServerPort();
        }

        [RelayCommand]
        private void SetItemsPerPage(string count)
        {
            if (int.TryParse(count, out int value))
            {
                ItemsPerPage = value;
            }
        }

        [RelayCommand]
        private async void TestConnection()
        {
            IsTestingConnection = true;
            // Simulate connection test
            ConnectionStatus = $"Đang kiểm tra kết nối tới {ServerHost}:{ServerPort}...";
            // In real app: async call to test server connection
            var result = await LocalSettingsHelper.TestConnectionAsync(ServerHost, ServerPort);
            ConnectionStatus = result.message;

            IsTestingConnection = false;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            // Lưu vào LocalSettings
            LocalSettingsHelper.SaveServerConfig(ServerHost, ServerPort);

            LastUpdated = $"Lần cập nhật cuối: {DateTime.Now:dd/MM/yyyy - HH:mm}";
            ConnectionStatus = "Đã lưu cấu hình!";
            AppInstance.Restart("");
        }

        [RelayCommand]
        private void CancelSettings()
        {
            LoadSettings(); 
            ConnectionStatus = string.Empty;
        }
    }
}
