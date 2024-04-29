using Requests;
using Shard_Downloader.Core;
using System;
using System.Linq;

namespace Shard_Downloader.MVVM.ViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        private string _value = (RequestHandler.MainRequestHandlers.First().StaticDegreeOfParallelism ?? RequestHandler.MainRequestHandlers.First().AutoParallelism.Invoke()).ToString();

        public string Value { get => _value; set => SetField(ref _value, value); }
        public RelayCommand CloseCommand { get; }
        public RelayCommand SaveCommand { get; }
        public event EventHandler? CloseWindowEvent;
        public event EventHandler? SaveSettingsEvent;
        public SettingsViewModel()
        {
            CloseCommand = new((o) => CloseWindowEvent?.Invoke(this, EventArgs.Empty));
            SaveCommand = new((o) => SaveSettingsEvent?.Invoke(this, EventArgs.Empty));
        }
    }
}
