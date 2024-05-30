using Microsoft.Extensions.DependencyInjection;
using Shard_Downloader.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Shard_Downloader.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            viewModel = App.Current.ServiceProvider.GetRequiredService<MainViewModel>();
            viewModel.SetHandler(hWnd);

            viewModel.OpenAddWindowEvent += OpenAddWindow;
            viewModel.OpenSettingsWindowEvent += OpenSettingsWindow;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void OpenSettingsWindow(object? sender, System.EventArgs e)
        {
            SettingsWindow s = new()
            {
                Owner = this
            };
            if (s.ShowDialog() == true)
                MainViewModel.SetParallelism(((SettingsViewModel)s.DataContext).Value);
        }

        private void OpenAddWindow(object? sender, System.EventArgs e)
        {
            AddWindow w = new()
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
                viewModel.AddRequest(((AddViewModel)w.DataContext).RequestData);

        }
    }
}
