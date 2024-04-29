using Requests;
using Requests.Options;
using Shard_Downloader.Core;
using Shard_Downloader.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shard_Downloader.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand AddCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public event EventHandler? OpenAddWindowEvent;
        public event EventHandler? OpenSettingsWindowEvent;
        public ObservableCollection<LoadRequestData> Requests { get; set; } = new();

        public MainViewModel()
        {
            AddCommand = new RelayCommand((o) => OpenAddWindowEvent?.Invoke(this, EventArgs.Empty));
            SettingsCommand = new RelayCommand((o) => OpenSettingsWindowEvent?.Invoke(this, EventArgs.Empty));
            CancelCommand = new RelayCommand((o) => Cancel());
            ClearCommand = new RelayCommand((o) => Clear());
        }

        private void Cancel()
        {
            foreach (LoadRequestData request in Requests)
                request.CancelCommand.Execute(null);
        }

        private void Clear()
        {
            List<LoadRequestData> remove = new();
            remove.AddRange(Requests.Where(req => req.State is RequestState.Compleated or RequestState.Failed or RequestState.Cancelled));
            foreach (LoadRequestData? req in remove)
                Requests.Remove(req);
        }

        internal void AddRequest(LoadRequestData data) => Requests.Add(data);

        internal void SetParallelism(string value)
        {
            if (string.IsNullOrEmpty(value))
                foreach (RequestHandler handler in RequestHandler.MainRequestHandlers)
                    handler.StaticDegreeOfParallelism = null;
            else
            if (byte.TryParse(value, out byte parallelism))
                foreach (RequestHandler handler in RequestHandler.MainRequestHandlers)
                    handler.StaticDegreeOfParallelism = parallelism;
        }
    }
}
