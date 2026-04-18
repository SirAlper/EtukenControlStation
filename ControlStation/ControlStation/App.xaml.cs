using ControlStation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using LibVLCSharp.Shared;

namespace ControlStation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            
            services.AddSingleton<MainWindow>();

            
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<ControlStation.Services.ConnectionsService>();
            services.AddSingleton<ControlStation.Services.RfService>();
            

            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

            Core.Initialize();

            mainWindow.DataContext = mainViewModel;
            
            mainWindow.Show();
        }
    }

}
