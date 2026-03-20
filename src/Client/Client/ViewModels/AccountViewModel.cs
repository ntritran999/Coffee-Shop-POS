using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Models;
using System;
using System.Collections.ObjectModel;

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
        public ObservableCollection<AccountListItem> Accounts { get; } = new();

        [ObservableProperty] private int _totalAccounts;

        // Add new account form fields
        [ObservableProperty] private string _newDisplayName = string.Empty;
        [ObservableProperty] private string _newUsername = string.Empty;
        [ObservableProperty] private string _newPassword = string.Empty;
        [ObservableProperty] private int _selectedRoleIndex = 0; // 0=Nhân viên, 1=Quản trị viên

        [ObservableProperty] private AccountListItem? _selectedAccount;

        public AccountViewModel()
        {
            LoadDummyData();
        }

        private void LoadDummyData()
        {
            Accounts.Clear();
            Accounts.Add(new AccountListItem
            {
                DisplayName = "Nguyễn Văn An",
                Username = "@an.nv",
                Role = "Quản trị viên",
                LastActive = "Vừa xong",
                IsAdmin = true
            });
            Accounts.Add(new AccountListItem
            {
                DisplayName = "Trần Thị Bình",
                Username = "@binh.tt",
                Role = "Nhân viên",
                LastActive = "2 giờ trước",
                IsAdmin = false
            });
            Accounts.Add(new AccountListItem
            {
                DisplayName = "Lê Hoàng Long",
                Username = "@long.lh",
                Role = "Nhân viên",
                LastActive = "Vừa xong",
                IsAdmin = false
            });
            Accounts.Add(new AccountListItem
            {
                DisplayName = "Phạm Minh Đức",
                Username = "@duc.pm",
                Role = "Nhân viên",
                LastActive = "Hôm qua",
                IsAdmin = false
            });
            Accounts.Add(new AccountListItem
            {
                DisplayName = "Hoàng Anh Tuấn",
                Username = "@tuan.ha",
                Role = "Quản trị viên",
                LastActive = "Vừa xong",
                IsAdmin = true
            });

            TotalAccounts = Accounts.Count;
        }

        [RelayCommand]
        private void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(NewDisplayName) || string.IsNullOrWhiteSpace(NewUsername))
                return;

            var isAdmin = SelectedRoleIndex == 1;
            Accounts.Add(new AccountListItem
            {
                DisplayName = NewDisplayName,
                Username = $"@{NewUsername}",
                Role = isAdmin ? "Quản trị viên" : "Nhân viên",
                LastActive = "Vừa xong",
                IsAdmin = isAdmin
            });

            TotalAccounts = Accounts.Count;
            ClearForm();
        }

        [RelayCommand]
        private void ClearForm()
        {
            NewDisplayName = string.Empty;
            NewUsername = string.Empty;
            NewPassword = string.Empty;
            SelectedRoleIndex = 0;
        }
    }
}
