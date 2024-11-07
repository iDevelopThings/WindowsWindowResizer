using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
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
        TryRelaunchWithAdmin();

        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new MainViewModel();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("AppState.json"));
        suspension.OnFrameworkInitializationCompleted();

        var state = MainViewModel.Get();
        new MainWindow {
            DataContext = state,
        }.Show();

        base.OnFrameworkInitializationCompleted();
    }

    public static void TryRelaunchWithAdmin() {
#if DEBUG
#else
        if (IsRunAsAdmin())
            return;

        var proc = new ProcessStartInfo();
        proc.UseShellExecute  = true;
        proc.WorkingDirectory = Environment.CurrentDirectory;
        proc.FileName         = Environment.GetCommandLineArgs()[0].Replace(".dll", ".exe");
        proc.Verb             = "runas";

        try {
            Process.Start(proc);
            Environment.Exit(0);
        }
        catch (Exception ex) {
            Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
        }
#endif

    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static bool IsRunAsAdmin() {
        var id        = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(id);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}