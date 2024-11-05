using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WinResizer.ViewModels;

namespace WinResizer;

public partial class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        AdminManager.TryRelaunchWithAdmin();

        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new MainViewModel();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("AppState.json"));
        suspension.OnFrameworkInitializationCompleted();

        var state = MainViewModel.Get();
        new MainWindow {
            DataContext = state,
        }.Show();

        base.OnFrameworkInitializationCompleted();


        /*if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }


        base.OnFrameworkInitializationCompleted();*/
    }
}