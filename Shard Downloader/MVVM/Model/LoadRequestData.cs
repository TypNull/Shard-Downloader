using DownloadAssistant.Options;
using DownloadAssistant.Request;
using Requests;
using Requests.Options;
using Shard_Downloader.Core;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shard_Downloader.MVVM.Model
{
    internal class LoadRequestData : ObservableObject
    {
        public RelayCommand PauseCommand { get; }
        public RelayCommand CancelCommand { get; }
        public string URL { get; set; } = string.Empty;
        public string InfoField => _request?.Filename ?? URL;

        public bool IsPaused { get => _isPaused; set => SetField(ref _isPaused, value); }
        private bool _isPaused;

        public RequestState State => _request?.State ?? RequestState.Waiting;

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
            CancelCommand = new RelayCommand((o) => Task.Run(() => _request?.Cancel()));
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
                RequestFailed = (IRequest s, HttpResponseMessage? message) => { Debug.WriteLine("Failed"); Debug.WriteLine(message?.StatusCode); OnPropertyChanged(nameof(State)); },
                RequestCancelled = (_) => OnPropertyChanged(nameof(State)),
                InfosFetched = (_) => OnPropertyChanged(nameof(InfoField)),
            };

            _request = new(URL, options);
            _request.StateChanged += (_, _) => OnPropertyChanged(nameof(State));
            _request.Progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object? sender, float e) => Progress = e;
    }
}
