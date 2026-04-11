using Client.Repositories;
using Client.Services;
using Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public static IServiceProvider? Services { get; private set; }
        public static IConfiguration? Configuration { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            var services = new ServiceCollection();


            services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient();
                // url and port must match the server's configuration
                client.BaseAddress = new Uri("http://localhost:5000/graphql");
                return client;
            });


            services.AddSingleton<ICategoryRepository, MockCategoryRepository>();
            services.AddSingleton<IProductRepository, ApiProductRepository>();
            services.AddSingleton<IBillInfoRepository, BillInfoRepository>();
            services.AddSingleton<IBillRepository, BillRepository>();
            services.AddSingleton<ITableRepository, TableRepository>();
            services.AddSingleton<IAccountRepository, ApiAccountRepository>();


            services.AddSingleton<AuthService>();
            services.AddSingleton<CategoryService>();
            services.AddSingleton<GeminiService>();
            services.AddSingleton<ProductService>();
            services.AddSingleton<BillService>();
            services.AddSingleton<TableService>();

            services.AddTransient<POSViewModel>();
            services.AddTransient<OrderViewModel>();
            services.AddTransient<TableViewModel>();
            services.AddTransient<CategoryViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<ProductViewModel>();
            services.AddTransient<DashboardViewModel>();

            Services = services.BuildServiceProvider();
            InitializeComponent();

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Trỏ đúng vào thư mục chạy của WinUI 3
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        public static Window? ActiveWindow { get; private set; }
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            ActiveWindow = _window;
            _window.Activate();
        }
    }
}
