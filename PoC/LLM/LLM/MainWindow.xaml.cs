using System;
using System.Collections.Generic;
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
using System.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LLM
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private LLMService llmService = new LLMService();
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void btnAsk_Click(object sender, RoutedEventArgs e)
        {
            string prompt = txtPrompt.Text;

            txtResult.Text = "Đang xử lý...";

            string answer = await llmService.AskAsync(prompt);

            txtResult.Text = answer;
        }
    }
}
