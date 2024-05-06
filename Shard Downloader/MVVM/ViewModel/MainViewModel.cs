using Microsoft.Win32;
using Requests;
using Requests.Options;
using Shard_Downloader.Core;
using Shard_Downloader.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Shard_Downloader.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private IntPtr _handler;

        public RelayCommand AddCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand ThemeCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public event EventHandler? OpenAddWindowEvent;
        public event EventHandler? OpenSettingsWindowEvent;

        public bool LightTheme { get => _lightTheme; set => SetField(ref _lightTheme, value); }
        private bool _lightTheme = true;
        public ObservableCollection<LoadRequestData> Requests { get; set; } = new();

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public MainViewModel()
        {
            AddCommand = new RelayCommand((o) => OpenAddWindowEvent?.Invoke(this, EventArgs.Empty));
            SettingsCommand = new RelayCommand((o) => OpenSettingsWindowEvent?.Invoke(this, EventArgs.Empty));
            CancelCommand = new RelayCommand((o) => Cancel());
            ClearCommand = new RelayCommand((o) => Clear());
            ThemeCommand = new RelayCommand((o) => { _ = DwmSetWindowAttribute(_handler, 20, new[] { 0 }, 4); });
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

        private static bool IsLightTheme()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            object? value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }

        internal void AddRequest(LoadRequestData data) => Requests.Add(data);

        internal static void SetParallelism(string value)
        {
            if (string.IsNullOrEmpty(value))
                foreach (RequestHandler handler in RequestHandler.MainRequestHandlers)
                    handler.StaticDegreeOfParallelism = null;
            else
            if (byte.TryParse(value, out byte parallelism))
                foreach (RequestHandler handler in RequestHandler.MainRequestHandlers)
                    handler.StaticDegreeOfParallelism = parallelism;
        }

        internal void SetHandler(IntPtr hWnd)
        {
            _handler = hWnd;
            if (!IsLightTheme())
            {
                LightTheme = false;
                App.Current.Resources.MergedDictionaries[0].Source = new Uri("/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute);
                _ = DwmSetWindowAttribute(hWnd, 20, new[] { 1 }, 4);
            }
        }
    }
}
