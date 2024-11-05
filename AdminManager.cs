using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Avalonia;

namespace WinResizer {

public class AdminManager {

    public static void TryRelaunchWithAdmin() {
#if DEBUG

#else
        if (IsRunAsAdmin())
            return;

        var proc = new ProcessStartInfo();
        proc.UseShellExecute = true;

        
        proc.WorkingDirectory = Environment.CurrentDirectory;
        proc.FileName         = Environment.GetCommandLineArgs()[0].Replace(".dll", ".exe");
        proc.Verb             = "runas";

        try {
            Process.Start(proc);

            // Launch a task to exit the current process after 1 second
            // Task.Delay(2000).ContinueWith(_ => Environment.Exit(0));

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

}