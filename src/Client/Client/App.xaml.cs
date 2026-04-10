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


            services.AddSingleton<ICategoryRepository, ApiCategoryRepository>();
            services.AddSingleton<IProductRepository, ApiProductRepository>();
            services.AddSingleton<ITableRepository, ApiTableRepository>();
            services.AddSingleton<IBillInfoRepository, BillInfoRepository>();
            services.AddSingleton<IBillRepository, BillRepository>();
            services.AddSingleton<IAccountRepository, ApiAccountRepository>();


            services.AddSingleton<AuthService>();
            services.AddSingleton<CategoryService>();
            services.AddSingleton<GeminiService>();
            services.AddSingleton<ProductService>();
            services.AddSingleton<BillService>();

            services.AddTransient<POSViewModel>();
            services.AddTransient<OrderViewModel>();
            services.AddTransient<TableViewModel>();
            services.AddTransient<CategoryViewModel>();
            services.AddTransient<ProductViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<AccountViewModel>();

            Services = services.BuildServiceProvider();
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
