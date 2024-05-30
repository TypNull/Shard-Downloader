using DownloadAssistant.Options;
using DownloadAssistant.Requests;
using Requests;
using Requests.Options;
using Shard_Downloader.Core;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace Shard_Downloader.MVVM.Model
{
    internal class LoadRequestData : ObservableObject
    {
        public RelayCommand PauseCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand RetryCommand { get; }
        public string URL { get; set; } = string.Empty;
        public string InfoField { get => _infoField; set => SetField(ref _infoField, value); }
        private string _infoField;

        public bool IsPaused { get => _isPaused; set => SetField(ref _isPaused, value); }
        private bool _isPaused;

        public RequestState State { get => _state; set => SetField(ref _state, value); }
        private RequestState _state = RequestState.Idle;

        public float Progress { get => _progress; set => SetField(ref _progress, value); }
        private float _progress;

        private LoadRequest? _request;

        public int Chunks { get; set; }
        public byte Attempts { get; set; }
        public long MaxBytes { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSecondHandler { get; set; }
        public bool IsAutostart { get; set; }
        public bool IsPriority { get; set; }
        public bool DeleteOnFailure { get; set; }
        public bool MergeWhileDownload { get; set; }

        public LoadRequestData()
        {
            _infoField = URL;
            PauseCommand = new RelayCommand((o) =>
            {
                if (State is RequestState.Compleated or RequestState.Failed)
                {
                    try
                    {
                        if (Path.GetDirectoryName(_request?.Destination) is string path && !string.IsNullOrEmpty(path))
                            CommandLine(path, $"start {path}");
                    }
                    catch (System.Exception ex) { Debug.WriteLine(ex); }
                    return;
                }
                IsPaused = !IsPaused;
                if (IsPaused)
                    _request?.Pause();
                else
                    _request?.Start();
            });
            CancelCommand = new RelayCommand((o) =>
            {
                _request?.Cancel();
            });
            RetryCommand = new RelayCommand((o) =>
            {
                if (State is RequestState.Cancelled or RequestState.Failed)
                {
                    IsAutostart = true;
                    CreateRequest();
                }
            });
        }

        private static void CommandLine(string workingDirectory, string Command)
        {
            ProcessStartInfo ProcessInfo;
            Process? Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + Command + " && exit")
            {
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process = Process.Start(ProcessInfo);
            Process?.WaitForExit();
        }

        public void CreateRequest()
        {
            IsPaused = !IsAutostart;
            LoadRequestOptions options = new()
            {
                Filename = Name,
                AutoStart = IsAutostart,
                Chunks = Chunks,
                DeleteTmpOnFailure = DeleteOnFailure,
                Priority = IsPriority ? RequestPriority.Normal : RequestPriority.Low,
                IsDownload = IsSecondHandler,
                MaxBytesPerSecond = MaxBytes,
                MergeWhileProgress = MergeWhileDownload,
                NumberOfAttempts = Attempts,
                RequestCompleated = (_, _) => OnPropertyChanged(nameof(State)),
                RequestStarted = (_) => { OnPropertyChanged(nameof(State)); },
                RequestFailed = (IRequest? s, HttpResponseMessage? message) => { Debug.WriteLine("Failed"); Debug.WriteLine(message?.StatusCode); OnPropertyChanged(nameof(State)); },
                RequestCancelled = (_) => OnPropertyChanged(nameof(State)),
                InfosFetched = (req) => InfoField = req?.Filename ?? URL,
            };

            _request = new(URL, options);
            _request.StateChanged += (_, state) => State = state;
            _request.Progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object? sender, float e) => Progress = e;
    }
}
