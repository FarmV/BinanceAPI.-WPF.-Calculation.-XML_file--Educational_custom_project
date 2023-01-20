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
        internal static bool StartAppDesigner = true ;

        [STAThread]
        static void Main(string[] args)
        {
            System.Windows.Application app = new System.Windows.Application();
            app.Startup += async (s, e) => await AppStartupAsync(args);
            app.Run();
        }

        private static async Task AppStartupAsync(string[] args) 
        {
            StartAppDesigner = false ;
            DataManagement dataManagement = await DataManagement.Create().ConfigureAwait(false);
            await OP.CreateMainWindow(new MainWindowViewModel(dataManagement)).ConfigureAwait(false);

            using IHost host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configuration) =>
            { configuration.Sources.Clear(); }).ConfigureServices((hostContext, container) =>
            {
                //container.AddSingleton<System.Windows.Application>(Services.CreateMainWindow(new MainWindowViewModel(dataManagement)));
                // container.BuildServiceProvider();
            }).Build();
            CancellationTokenSource cts = new CancellationTokenSource();
            IServiceProvider r11 = host.Services;
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                System.Windows.Application.Current.MainWindow.Closed += (_, _) => cts.Cancel();
                System.Windows.Application.Current.MainWindow.Show();
            });
            await host.RunAsync(cts.Token);
        }
    }

    internal static partial class OP
    {
        internal static async Task CreateMainWindow(MainWindowViewModel viewModel)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MainWindow mainWindow = new MainWindow(viewModel);
                //WindowInteropHelper interopHelper = new WindowInteropHelper(mainWindow);
                //interopHelper.EnsureHandle();
            }).Task.ConfigureAwait(false);
        }


    }
}