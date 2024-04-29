using Shard_Downloader.MVVM.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Shard_Downloader.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        public MainWindow()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            _ = DwmSetWindowAttribute(hWnd, 20, new[] { 1 }, 4);
            ((MainViewModel)DataContext).OpenAddWindowEvent += OpenAddWindow;
            ((MainViewModel)DataContext).OpenSettingsWindowEvent += OpenSettingsWindow; ;
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
