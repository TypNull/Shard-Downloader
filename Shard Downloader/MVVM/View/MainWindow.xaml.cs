using Shard_Downloader.MVVM.ViewModel;
using System.Windows;

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
            ((MainViewModel)DataContext).OpenAddWindowEvent += OpenAddWindow;
            ((MainViewModel)DataContext).OpenSettingsWindowEvent += OpenSettingsWindow; ;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void OpenSettingsWindow(object? sender, System.EventArgs e)
        {
            SettingsWindow s = new();
            s.Owner = this;
            if (s.ShowDialog() == true)
                ((MainViewModel)DataContext).SetParallelism(((SettingsViewModel)s.DataContext).Value);
        }

        private void OpenAddWindow(object? sender, System.EventArgs e)
        {
            AddWindow w = new();
            w.Owner = this;
            if (w.ShowDialog() == true)
                ((MainViewModel)DataContext).AddRequest(((AddViewModel)w.DataContext).RequestData);

        }
    }
}
