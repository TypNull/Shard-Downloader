using Shard_Downloader.MVVM.ViewModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Shard_Downloader.MVVM.View
{
    /// <summary>
    /// Interaktionslogik für AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
            ((AddViewModel)DataContext).CloseAddWindowEvent += CloseAddWindow;
            ((AddViewModel)DataContext).ClickDownloadEvent += Download;
        }

        private void CloseAddWindow(object? sender, System.EventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void Download(object? sender, System.EventArgs e)
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
