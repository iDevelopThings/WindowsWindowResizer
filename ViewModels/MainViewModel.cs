using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace WinResizer.ViewModels;

[DataContract]
public class SavedWindowConfig : ReactiveObject {
    private Rect _rect = new();
    [DataMember]
    public Rect Rect {
        get => _rect;
        set => this.RaiseAndSetIfChanged(ref _rect, value);
    }
    private bool _isResizable;
    [DataMember]
    public bool IsResizable {
        get => _isResizable;
        set => this.RaiseAndSetIfChanged(ref _isResizable, value);
    }

    private string? _processName;
    [DataMember]
    public string? ProcessName {
        get => _processName;
        set => this.RaiseAndSetIfChanged(ref _processName, value);
    }


    public SavedWindowConfig() { }
    public SavedWindowConfig(Window window) {
        ProcessName = window.Description;
        Rect        = window.Dimensions;
    }
}

[DataContract]
public class MainViewModel : ReactiveObject {
    private ObservableCollection<SavedWindowConfig> _savedWindows = new();
    [DataMember]
    public ObservableCollection<SavedWindowConfig> SavedWindows {
        get => _savedWindows;
        set => this.RaiseAndSetIfChanged(ref _savedWindows, value);
    }

    private ObservableCollection<Window> _windows = new();

    [IgnoreDataMember]
    public ObservableCollection<Window> Windows {
        get => _windows;
        set => this.RaiseAndSetIfChanged(ref _windows, value);
    }

    private Window? _selectedWindow;

    [IgnoreDataMember]
    public Window? SelectedWindow {
        get => _selectedWindow;
        set {
            this.RaiseAndSetIfChanged(ref _selectedWindow, value);
            this.RaisePropertyChanged(nameof(IsWindowSelected));
            this.RaisePropertyChanged(nameof(SelectedWindowDescription));
            this.RaisePropertyChanged(nameof(IsSavedWindow));

            if (value != null) {
                WindowWidth  = value.Dimensions.Width;
                WindowHeight = value.Dimensions.Height;
            }
        }
    }

    public bool    IsSavedWindow             => SelectedWindow != null && GetSavedWindow(SelectedWindow) != null;
    public bool    IsWindowSelected          => SelectedWindow != null;
    public string? SelectedWindowDescription => SelectedWindow?.Description;

    private double _windowWidth;

    [IgnoreDataMember]
    public double WindowWidth {
        get => _windowWidth;
        set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
    }

    private double _windowHeight;

    [IgnoreDataMember]
    public double WindowHeight {
        get => _windowHeight;
        set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    }


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

    public MainViewModel() {
        ReloadWindowsCommand = ReactiveCommand.Create(ReloadActiveWindows);

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
        /*WindowSelectedCommand = ReactiveCommand.Create(() => {
            if (SelectedWindow != null) {
                Console.WriteLine($"Selected window: {SelectedWindow.Description}");
            }
        });*/
    }
    private void ResizeWindow() {
        
        if (SelectedWindow == null) {
            return;
        }

        var newRect = new Rect(
            SelectedWindow!.Dimensions.Left,
            SelectedWindow.Dimensions.Top,
            (int) _windowWidth,
            (int) _windowHeight
        );

        try {
            WindowService.ResizeWindow(SelectedWindow, newRect);
            SelectedWindow.Dimensions = WinApiService.GetWindowRect(SelectedWindow.WindowHandle);
            UpdateWindowConfig(SelectedWindow);
        }
        catch (Exception e) {
            Console.WriteLine($"Error resizing window: {e.Message}");
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

    public static MainViewModel Get() {
        return RxApp.SuspensionHost.GetAppState<MainViewModel>();
    }
    
    public Task ReloadWindowsAsync() {
        return Task.Run(ReloadActiveWindows);
    }

    public void ReloadActiveWindows() {
        var prevSelectedWindow = SelectedWindow;

        Windows.Clear();

        foreach (var process in WindowService.GetProcesses()) {
            try {
                var dimensions = WinApiService.GetWindowRect(process.MainWindowHandle);
                if (dimensions.IsEmpty) {
                    continue; // Has a 0 by 0 window, and not meant to display
                }

                var iconBitmap = WindowService.TryGetProcessIcon(process);


                string description;
                try {
                    description = process.MainModule?.FileVersionInfo.FileDescription ?? string.Empty;
                    if (string.IsNullOrEmpty(description)) {
                        description = process.ProcessName ?? string.Empty;
                    }
                }
                catch (Win32Exception e) {
                    // check if `Access Denied` error
                    if (e.NativeErrorCode == 5) {
                        description = process.ProcessName ?? string.Empty;
                    } else {
                        throw;
                    }
                }

                Windows.Add(new Window() {
                    Id           = process.Id.ToString(),
                    Name         = process.ProcessName,
                    WindowHandle = process.MainWindowHandle,
                    Description  = description,
                    Icon         = iconBitmap,
                    Dimensions   = dimensions,
                });
            }
            catch (Win32Exception e) {
                Console.WriteLine($"Error getting window for process {process.ProcessName}: {e.Message}");
                // Do nothing
            }
        }

        if (prevSelectedWindow != null) {
            SelectedWindow = Windows.FirstOrDefault(w => w.Equals(prevSelectedWindow));
        }
    }

    public void UpdateWindowConfig(Window window, Action<SavedWindowConfig>? updateAction = null) {
        var savedWindow = GetOrCreateSavedWindow(window);
        if (savedWindow != null) {
            savedWindow.Rect = window.Dimensions;

            if (updateAction != null) {
                updateAction(savedWindow);
            }
        }
    }
    public SavedWindowConfig? GetSavedWindow(Window? window) {
        return SavedWindows.FirstOrDefault(
            w => w.ProcessName == window?.Name
        );
    }
    public SavedWindowConfig? GetOrCreateSavedWindow(Window window) {
        var w = GetSavedWindow(window);

        if (w == null) {
            w = new SavedWindowConfig(window);
            SavedWindows.Add(w);
            // UpdateWindowConfig(window);
        }

        return w;
    }

    public void AddAsSavedWindow(Window window) {
        var w = GetOrCreateSavedWindow(window);
        this.RaisePropertyChanged(nameof(IsSavedWindow));
    }
}