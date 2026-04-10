using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Models;
using Client.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class AccountListItem : ObservableObject
    {
        [ObservableProperty] private string _displayName = string.Empty;
        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _role = "Nhân viên";
        [ObservableProperty] private string _lastActive = string.Empty;
        [ObservableProperty] private bool _isAdmin;
    }

    public partial class AccountViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        public ObservableCollection<AccountListItem> Accounts { get; } = new();

        [ObservableProperty] private int _totalAccounts;

        [ObservableProperty] private string _newDisplayName = string.Empty;
        [ObservableProperty] private string _newUsername = string.Empty;
        [ObservableProperty] private string _newPassword = string.Empty;
        [ObservableProperty] private int _selectedRoleIndex = 0;

        [ObservableProperty] private AccountListItem? _selectedAccount;
        [ObservableProperty] private string _statusMessage = string.Empty; 

        public AccountViewModel(AuthService authService)
        {
            _authService = authService;
            LoadAccountsAsync();
        }

        private async void LoadAccountsAsync()
        {
            Accounts.Clear();
            var accountList = await _authService.GetAllAccounts();

            foreach (var acc in accountList)
            {
                bool isAdmin = acc.Role == "Manager";
                Accounts.Add(new AccountListItem
                {
                    DisplayName = acc.DisplayName,
                    Username = acc.Username,
                    Role = acc.Role,
                    LastActive = "N/A", 
                    IsAdmin = isAdmin
                });
            }
            TotalAccounts = Accounts.Count;
        }

        // Sửa thành Async để ghi xuống txt
        [RelayCommand]
        private async Task SaveAccountAsync()
        {
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NewDisplayName) ||
                string.IsNullOrWhiteSpace(NewUsername) ||
                string.IsNullOrWhiteSpace(NewPassword))
            {
                StatusMessage = "Vui lòng nhập đầy đủ tên, username và mật khẩu.";
                return;
            }

            var role = SelectedRoleIndex == 1 ? "Manager" : "Cashier";

            try
            {
                await _authService.Register(NewUsername, NewPassword, NewDisplayName, role);

                // Load lại list trên UI
                LoadAccountsAsync();
                ClearForm();
                StatusMessage = "Thêm tài khoản thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi: {ex.Message}";
            }
        }

        [RelayCommand]
        private void ClearForm()
        {
            NewDisplayName = string.Empty;
            NewUsername = string.Empty;
            NewPassword = string.Empty;
            SelectedRoleIndex = 0;
            StatusMessage = string.Empty;
        }
    }
}