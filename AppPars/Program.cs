using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

namespace AppPars
{
    internal class Program
    {
        internal static string RelativePathSaveData = $"{Path.GetDirectoryName(Environment.ProcessPath)}\\results\\";
        internal static bool StartAppDesigner = true;

        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Application app = new System.Windows.Application();
            app.Startup += async (_, _) => await AppStartupAsync(args);
            app.Run();
        }

        private static async Task AppStartupAsync(string[] args)
        {
            StartAppDesigner = false;
            using IHost host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configuration) =>
            { configuration.Sources.Clear(); }).ConfigureServices((_, container) =>
            {
                container.AddSingleton(Services.CreateMainWindow(Services.CreateMainWindowViewModel()));
            }).Build();
            CancellationTokenSource cts = new CancellationTokenSource();
            IServiceProvider serviceProvider = host.Services;
            Task<MainWindow> createMainWindow = serviceProvider.GetRequiredService<Task<MainWindow>>();
            MainWindow mainWindow = await createMainWindow.ConfigureAwait(false);
            await mainWindow.Dispatcher.InvokeAsync(() =>
            {
                mainWindow.Closed += (_, _) => cts.Cancel();
                mainWindow.Show();
            });            
            await host.RunAsync(cts.Token);
        }
    }

    internal static partial class Services
    {
        internal static async Task<MainWindow> CreateMainWindow(MainWindowViewModel viewModel)
        {
            MainWindow? mainWindow = null;
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => { mainWindow = new MainWindow(viewModel); }).Task.ConfigureAwait(false);
            return mainWindow ?? throw new NullReferenceException();
        }
        internal static MainWindowViewModel CreateMainWindowViewModel() => new MainWindowViewModel(DataManagement.Create());                    
    }
}