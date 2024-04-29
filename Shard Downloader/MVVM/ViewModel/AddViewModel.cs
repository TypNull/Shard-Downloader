using Shard_Downloader.Core;
using Shard_Downloader.MVVM.Model;
using System;
using System.Threading.Tasks;

namespace Shard_Downloader.MVVM.ViewModel
{
    internal class AddViewModel : ObservableObject

    {
        private string _url = string.Empty;
        private string _chunks = string.Empty;
        private string _attemps = "3";
        private string _maxBytes = string.Empty;
        private string _name = string.Empty;
        private bool _isSecondHandler;
        private bool _isAutostart = true;
        private bool _isPriority;
        private bool _deleteOnFailure;
        private bool _mergeWhileDownload = true;

        public RelayCommand CloseCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public LoadRequestData RequestData { get; private set; } = new();

        public string URL { get => _url; set => SetField(ref _url, value); }
        public string Chunks { get => _chunks; set => SetField(ref _chunks, value); }
        public string Attempts { get => _attemps; set => SetField(ref _attemps, value); }
        public string MaxBytes { get => _maxBytes; set => SetField(ref _maxBytes, value); }
        public string Name { get => _name; set => SetField(ref _name, value); }
        public bool IsSecondHandler { get => _isSecondHandler; set => SetField(ref _isSecondHandler, value); }
        public bool IsAutostart { get => _isAutostart; set => SetField(ref _isAutostart, value); }
        public bool IsPriority { get => _isPriority; set => SetField(ref _isPriority, value); }
        public bool DeleteOnFailure { get => _deleteOnFailure; set => SetField(ref _deleteOnFailure, value); }
        public bool MergeWhileDownload { get => _mergeWhileDownload; set => SetField(ref _mergeWhileDownload, value); }

        public event EventHandler? CloseAddWindowEvent;
        public event EventHandler? ClickDownloadEvent;

        public AddViewModel()
        {
            CloseCommand = new((o) => CloseAddWindowEvent?.Invoke(this, EventArgs.Empty));
            DownloadCommand = new((o) => AddToDownload());
        }

        private void AddToDownload()
        {
            if (Uri.TryCreate(URL, UriKind.Absolute, out Uri? uriResult) && uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                RequestData.URL = URL;
                RequestData.IsSecondHandler = IsSecondHandler;
                RequestData.MaxBytes = long.TryParse(MaxBytes, out long maxbytes) ? maxbytes : 0;
                RequestData.IsPriority = IsPriority;
                RequestData.DeleteOnFailure = DeleteOnFailure;
                RequestData.IsAutostart = IsAutostart;
                RequestData.Attempts = byte.TryParse(Attempts, out byte attempts) ? attempts : (byte)3;
                RequestData.Chunks = int.TryParse(Chunks, out int chunks) ? chunks : 0;
                RequestData.MergeWhileDownload = MergeWhileDownload;
                RequestData.Name = Name;
                ClickDownloadEvent?.Invoke(this, EventArgs.Empty);

                Task.Run(() => RequestData.CreateRequest());
            }
        }
    }
}
