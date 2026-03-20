using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Client.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        // Display settings
        [ObservableProperty] private int _itemsPerPage = 10;
        [ObservableProperty] private bool _rememberLastScreen = true;

        // Connection settings
        [ObservableProperty] private string _serverHost = "192.168.1.100";
        [ObservableProperty] private string _serverPort = "8080";

        // Connection test status
        [ObservableProperty] private string _connectionStatus = string.Empty;
        [ObservableProperty] private bool _isTestingConnection;

        // Last updated label
        [ObservableProperty] private string _lastUpdated = "Lần cập nhật cuối: 15/10/2023 - 14:30";

        public SettingsViewModel()
        {
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
        private void TestConnection()
        {
            IsTestingConnection = true;
            // Simulate connection test
            ConnectionStatus = $"Đang kiểm tra kết nối tới {ServerHost}:{ServerPort}...";
            // In real app: async call to test server connection
            ConnectionStatus = "✓ Kết nối thành công!";
            IsTestingConnection = false;
        }

        [RelayCommand]
        private void SaveSettings()
        {
            // In real app: persist settings to local storage
            LastUpdated = $"Lần cập nhật cuối: {DateTime.Now:dd/MM/yyyy - HH:mm}";
        }

        [RelayCommand]
        private void CancelSettings()
        {
            // In real app: reload settings from storage
            ServerHost = "192.168.1.100";
            ServerPort = "8080";
            ItemsPerPage = 10;
            RememberLastScreen = true;
        }
    }
}
