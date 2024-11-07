using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WinResizer.ViewModels;

#if DEBUG
using Avalonia.Diagnostics;
using Avalonia.Input;
#endif

namespace WinResizer;

public partial class MainWindow : ReactiveWindow<MainViewModel> {

    public MainWindow() {
        Width     = 650;
        Height    = 400;
        CanResize = true;

        this.WhenActivated(disposables => {
#if DEBUG
            this.AttachDevTools(new DevToolsOptions() {
                Gesture           = new KeyGesture(Key.F1),
                ShowAsChildWindow = false,
            });
#endif
            ViewModel = MainViewModel.Get();
            ViewModel?.ReloadWindowsAsync();

            // disposables.Add(
            //     DispatcherTimer.Run(
            //         () => {
            //             // ViewModel?.ReloadActiveWindows();
            //             ViewModel?.ReloadWindowsAsync();//.ConfigureAwait(false);
            //             return true;
            //         },
            //         TimeSpan.FromSeconds(2)
            //     ));

            // Create & run a "timer" to update the list of active windows
            // Observable.Interval(TimeSpan.FromSeconds(2))
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(_ => ViewModel?.ReloadWindowsAsync())
            //    .DisposeWith(disposables);
        });

#if DEBUG
        InitializeComponent(true, false);
#else
        InitializeComponent(true);
#endif


        // ViewModel.ReloadActiveWindows();
        // ViewModel.PropertyChanged += (sender, args) => { };

    }
    private void Processes_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (ViewModel?.SelectedWindow == null)
            return;
        // ViewModel.WindowHeight = ViewModel.SelectedWindow.Dimensions.Height;
        // ViewModel.WindowWidth  = ViewModel.SelectedWindow.Dimensions.Width;

    }
}