using Shard_Downloader.MVVM.ViewModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Shard_Downloader.MVVM.View
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ((SettingsViewModel)DataContext).CloseWindowEvent += CloseWindow;
            ((SettingsViewModel)DataContext).SaveSettingsEvent += SaveSettings;
        }

        private void CloseWindow(object? sender, System.EventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void SaveSettings(object? sender, System.EventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
