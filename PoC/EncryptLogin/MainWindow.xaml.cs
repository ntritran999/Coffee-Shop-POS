using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EncryptLogin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"{txtUsername.Text} {txtPassword.Password}");
            User user = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Password
            };
            if (user.Username.Equals("") || user.Password.Equals("")) return;



            PasswordEncoder passwordEncoder = new PasswordEncoder();

            string hashedPassword = passwordEncoder.HashPassword(user.Password);

            Debug.WriteLine($"Username: {user.Password}, Hashed Password: {hashedPassword}");
            txtHash.Text = hashedPassword;

            User storedUser = new User
            {
                Username = "test",
                Password = passwordEncoder.HashPassword("password")
            };

            bool isPasswordValid = passwordEncoder.VerifyPassword(user.Password, storedUser.Password);

            if (isPasswordValid)
            {
                Debug.WriteLine("Login successful!");
                txtStatus.Text = "Login successful!";
            }
            else
            {
                Debug.WriteLine("Invalid username or password.");
                txtStatus.Text = "Invalid username or password.";
            }
        }
    }
}
