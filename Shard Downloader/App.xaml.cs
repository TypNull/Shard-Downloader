using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shard_Downloader.MVVM.View;
using Shard_Downloader.MVVM.ViewModel;
using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Shard_Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly ServiceProvider _serviceProvider;
        public ServiceProvider ServiceProvider => _serviceProvider;
        internal static new App Current => (App)Application.Current;

        private readonly ILogger<App> _logger;

        public App(ILoggerFactory loggerF)
        {
            _logger = loggerF.CreateLogger<App>();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(provider => new MainWindow { DataContext = provider.GetRequiredService<MainViewModel>() });
            services.AddSingleton<MainViewModel>();
            _serviceProvider = services.BuildServiceProvider();
        }

        public static void Log(string information, LogLevel logLevel = LogLevel.Information) => Current._logger.Log(logLevel, information);


        public static void Log<T>(string information, LogLevel logLevel = LogLevel.Information, bool time = false, [CallerMemberName] string callerName = "", [CallerLineNumber] long callerLineNumber = 0) =>
            Current._logger.Log(logLevel, $"Time: {(time ? DateTime.Now.TimeOfDay : "")} Class: {typeof(T).FullName} Method:{callerName} Line: {callerLineNumber} »»  {information} ");


        protected override void OnStartup(StartupEventArgs e)
        {
            Log<App>("Start Application", time: true);
            base.OnStartup(e);
            _serviceProvider.GetRequiredService<MainWindow>().Show();
            Log<App>("Show MainWindow", time: true);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _serviceProvider.GetRequiredService<MainViewModel>().SaveRequests();
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.0.0")]
        public static void Main()
        {
            IHost host = Host.CreateDefaultBuilder().ConfigureServices(services => services.AddSingleton<App>()).Build();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                ILoggerFactory? loggerFactory = host.Services.GetService<ILoggerFactory>();
                if (loggerFactory == null)
                    return;
                ILogger<App> logger = loggerFactory.CreateLogger<App>();
                logger.LogError(args.ExceptionObject as Exception, "Application exited because of an unhandled exception");
            };
            App? app = host.Services.GetService<App>();
            app?.InitializeComponent();
            app?.Run();
        }
    }
}
