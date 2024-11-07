using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ReactiveUI;

namespace WinResizer.ViewModels;

[DataContract]
public class MainViewModel : BaseViewModel {
    private ObservableCollection<SavedWindowConfig> _savedWindows = new();
    [DataMember]
    public ObservableCollection<SavedWindowConfig> SavedWindows {
        get => _savedWindows;
        set => this.RaiseAndSetIfChanged(ref _savedWindows, value);
    }

    private ObservableCollection<SystemWindow> _windows = new();

    [IgnoreDataMember]
    public ObservableCollection<SystemWindow> Windows {
        get => _windows;
        set => this.RaiseAndSetIfChanged(ref _windows, value);
    }

    private SystemWindow? _selectedWindow;

    [IgnoreDataMember]
    public SystemWindow? SelectedWindow {
        get => _selectedWindow;
        set {
            var prevWindow = _selectedWindow;
            var newWindow  = this.RaiseAndSetIfChanged(ref _selectedWindow, value);

            if (prevWindow != null && newWindow != null && !prevWindow.Equals(newWindow)) {
                prevWindow.IsSelected = false;
                newWindow.IsSelected  = true;
            }

            this.RaisePropertyChanged(nameof(IsWindowSelected));
            this.RaisePropertyChanged(nameof(SelectedWindowDescription));
            this.RaisePropertyChanged(nameof(IsSavedWindow));
        }
    }

    public bool    IsSavedWindow             => SelectedWindow != null && GetSavedWindow(SelectedWindow) != null;
    public bool    IsWindowSelected          => SelectedWindow != null;
    public string? SelectedWindowDescription => SelectedWindow?.Description;


    [IgnoreDataMember]
    public ReactiveCommand<Unit, Unit> ReloadWindowsCommand { get; }
    [IgnoreDataMember]
    public ReactiveCommand<Unit, Unit> ResizeWindowCommand { get; }
    [IgnoreDataMember]
    public ReactiveCommand<Unit, Unit> SetResizableCommand { get; }
    [IgnoreDataMember]
    public ReactiveCommand<Unit, Unit> SaveWindowCommand { get; }
    [IgnoreDataMember]
    public ReactiveCommand<Unit, Unit> RemoveSavedWindowCommand { get; }

    public static MainViewModel Get() => RxApp.SuspensionHost.GetAppState<MainViewModel>();

    public MainViewModel() {
        ReloadWindowsCommand = ReactiveCommand.CreateFromTask(ReloadWindowsAsync);

        ResizeWindowCommand = ReactiveCommand.Create(ResizeWindow);
        SetResizableCommand = ReactiveCommand.Create(SetResizable);

        SaveWindowCommand = ReactiveCommand.Create(() => {
            if (SelectedWindow != null) {
                AddAsSavedWindow(SelectedWindow);
            }
        });

        RemoveSavedWindowCommand = ReactiveCommand.Create(() => {
            if (SelectedWindow == null)
                return;
            var savedWindow = GetSavedWindow(SelectedWindow);
            if (savedWindow == null)
                return;

            SavedWindows.Remove(savedWindow);
            this.RaisePropertyChanged(nameof(IsSavedWindow));
        });

        Observable.Interval(TimeSpan.FromSeconds(5))
           .StartWith(0)
           .ObserveOn(RxApp.MainThreadScheduler)
           .Subscribe(_ => RefreshProcesses());
    }

    private void ResizeWindow() {
        if (SelectedWindow?.Resize() == true) {
            UpdateWindowConfig(SelectedWindow);
        }
    }

    private void SetResizable() {
        if (SelectedWindow == null)
            return;

        WindowService.SetResizableBorder(SelectedWindow);
        UpdateWindowConfig(SelectedWindow, (w) => {
            w.IsResizable = true;
        });
    }


    public Task ReloadWindowsAsync() => Task.Run(RefreshProcesses);

    public void RefreshProcesses() {
        var prevSelectedWindow = SelectedWindow;

        var existingWindowList = Windows.ToList();
        var existingIds        = new HashSet<int>(existingWindowList.Select(p => p.Id));
        var newProcessesList   = WindowService.GetProcesses().ToList();

        // Remove closed processes
        foreach (var process in existingWindowList) {
            if (newProcessesList.Any(p => p.Id == process.Id))
                continue;
            Windows.Remove(process);

            if (SelectedWindow != null && SelectedWindow.Id == process.Id) {
                SelectedWindow = null;
            }

        }

        // Add new processes
        foreach (var process in newProcessesList) {
            if (existingIds.Contains(process.Id))
                continue;

            try {
                Windows.Add(new SystemWindow(process));
            }
            catch (Win32Exception e) {
                Console.WriteLine($"Error getting window for process {process.ProcessName}: {e.Message}");
            }
        }

        SelectedWindow = prevSelectedWindow != null
            ? Windows.FirstOrDefault(w => w.Equals(prevSelectedWindow))
            : Windows.FirstOrDefault();
    }

    public void UpdateWindowConfig(SystemWindow systemWindow, Action<SavedWindowConfig>? updateAction = null) {
        var savedWindow = GetOrCreateSavedWindow(systemWindow);
        if (savedWindow != null) {
            savedWindow.Rect = systemWindow.Dimensions;

            if (updateAction != null) {
                updateAction(savedWindow);
            }
        }
    }
    public SavedWindowConfig? GetSavedWindow(SystemWindow? window) {
        return SavedWindows.FirstOrDefault(
            w => w.ProcessName == window?.Name
        );
    }
    public SavedWindowConfig? GetOrCreateSavedWindow(SystemWindow systemWindow) {
        var w = GetSavedWindow(systemWindow);

        if (w == null) {
            w = new SavedWindowConfig(systemWindow);
            SavedWindows.Add(w);
            // UpdateWindowConfig(window);
        }

        return w;
    }

    public void AddAsSavedWindow(SystemWindow systemWindow) {
        GetOrCreateSavedWindow(systemWindow);
        this.RaisePropertyChanged(nameof(IsSavedWindow));
    }


}