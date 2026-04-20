using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly string _prefsFilePath;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _errorMessage = string.Empty;
        [ObservableProperty] private bool _isRememberMe;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            _prefsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "login_prefs.txt");
            LoadSavedCredentials();
            _ = AutoLoginIfSavedAsync();
        }

        private async Task AutoLoginIfSavedAsync()
        {
            if (IsRememberMe && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                // Tạo độ trễ (delay) khoảng 800 mili-giây. 
                await Task.Delay(800);

                // Gọi command đăng nhập
                await LoginAsync();
            }
        }

        private void LoadSavedCredentials()
        {
            try
            {
                if (File.Exists(_prefsFilePath))
                {
                    var lines = File.ReadAllLines(_prefsFilePath);
                    if (lines.Length == 2)
                    {
                        Username = lines[0];
                        Password = lines[1];
                        IsRememberMe = true; // Tự động tick vào ô Checkbox
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đọc file cấu hình: {ex.Message}");
            }
        }

        private void SaveCredentials()
        {
            try
            {
                if (IsRememberMe)
                {
                    // Lưu 2 dòng: dòng 1 là Username, dòng 2 là Password
                    File.WriteAllLines(_prefsFilePath, new[] { Username, Password });
                }
                else
                {
                    // Nếu người dùng bỏ tick, xóa file đi để quên đăng nhập
                    if (File.Exists(_prefsFilePath))
                    {
                        File.Delete(_prefsFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi ghi file cấu hình: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            try
            {
                var account = await _authService.Login(Username, Password);

                if (account != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Đăng nhập thành công: {account.DisplayName}");

                    // 1. Lưu phiên làm việc hiện tại
                    SessionManager.CurrentAccount = account;

                    // 2. Kích hoạt logic Nhớ Đăng Nhập
                    SaveCredentials();

                    // 3. Chuyển sang màn hình chính
                    MainWindow.AppFrame.Navigate(typeof(Client.Views.MainPage));
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }
        }
    }
}