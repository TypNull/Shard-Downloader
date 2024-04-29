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
        public MainWindow()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();

            ((MainViewModel)DataContext).SetHandler(hWnd);

            ((MainViewModel)DataContext).OpenAddWindowEvent += OpenAddWindow;
            ((MainViewModel)DataContext).OpenSettingsWindowEvent += OpenSettingsWindow;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void OpenSettingsWindow(object? sender, System.EventArgs e)
        {
            SettingsWindow s = new()
            {
                Owner = this
            };
            if (s.ShowDialog() == true)
                ((MainViewModel)DataContext).SetParallelism(((SettingsViewModel)s.DataContext).Value);
        }

        private void OpenAddWindow(object? sender, System.EventArgs e)
        {
            AddWindow w = new()
            {
                Owner = this
            };
            if (w.ShowDialog() == true)
                ((MainViewModel)DataContext).AddRequest(((AddViewModel)w.DataContext).RequestData);

        }
    }
}
