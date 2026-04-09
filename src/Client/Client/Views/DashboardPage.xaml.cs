using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Client.ViewModels;
using Client.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; set; } = new ();
        public DashboardPage()
        {
            InitializeComponent();
        }
    }

    public class TopSellingProductItem
    {
        public Product product { get; set; } = default!;
        public int totalSold { get; set; }
    }

    public class RecentOrderItem
    {
        public int BillID { get; set; }
        public DateTime DateCheckIn { get; set; }
        public int? TableID { get; set; }
        public int Status { get; set; }

        public string StatusText => Status == 1 ? "Đã thanh toán" : "Chưa thanh toán";
        public double TotalAmount { get; set; }
        public List<BillInfo> Items { get; set; } = new();
    }
}