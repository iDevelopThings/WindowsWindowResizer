using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Platform;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace WinResizer {

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class WindowService {

    // public static ImageList Icons { get; set; } = new ImageList() {
    // ImageSize = new Size(22, 22),
    // };

    public static IEnumerable<Process> GetProcesses() {
        return Process.GetProcesses()
           .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
           .Where(p => p.MainWindowHandle != IntPtr.Zero);
    }


    /*public static IEnumerable<Window> GetActiveWindows() {
        var windows = new List<Window>();

        foreach (var process in GetProcesses()) {
            try {
                var dimensions = WinApiService.GetWindowRect(process.MainWindowHandle);
                if (dimensions.IsEmpty) {
                    continue; // Has a 0 by 0 window, and not meant to display
                }

                var iconBitmap = TryGetProcessIcon(process);
                // if (iconBitmap != null) {
                // Icons.Images.Add(process.ProcessName, iconBitmap);
                // }

                string description;
                try {
                    description = process.MainModule?.FileVersionInfo.FileDescription ?? string.Empty;
                }
                catch (Win32Exception e) {
                    // check if `Access Denied` error
                    if (e.NativeErrorCode == 5) {
                        description = process.ProcessName ?? string.Empty;
                    } else {
                        throw;
                    }
                }

                windows.Add(new Window() {
                    Id           = process.Id,
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

        return windows.OrderBy(w => w.Description).ToList();
    }*/

    public static Bitmap? TryGetProcessIcon(Process process) {
        try {
            if (process.MainModule?.FileName != null && File.Exists(process.MainModule.FileName)) {
                var icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);

                if (icon == null) {
                    return null;
                }

                var bitMap = icon.ToBitmap();

                using var memory = new MemoryStream();

                bitMap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                //AvIrBitmap is our new Avalonia compatible image. You can pass this to your view
                return new Bitmap(memory);

                // convert bitMap into a stream
                // var stream = new MemoryStream();
                // bitMap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                // return new Bitmap(stream);
            }
        }
        catch (Win32Exception e) {
            Console.WriteLine($"Error getting icon for process {process.ProcessName}: {e.Message}");
        }
        return null;
    }


    public static void ResizeWindow(Window window, Rect resolution) {
        var newWidth    = resolution.Right == 0 ? window.Dimensions.Right : resolution.Right;
        var newHeight   = resolution.Bottom == 0 ? window.Dimensions.Bottom : resolution.Bottom;
        var newPosition = new Rect {Left = window.Dimensions.Left, Top = window.Dimensions.Top, Right = newWidth, Bottom = newHeight};

        WinApiService.SetWindowPos(window.WindowHandle, newPosition, SwpType.NoMove | SwpType.NoActive | SwpType.NoZOrder);
    }

    public static void RelocateWindow(Window window, Rect location) {
        WinApiService.SetWindowPos(window.WindowHandle, location, SwpType.NoSize | SwpType.NoActive | SwpType.NoZOrder);
    }
    
    public static bool SetResizableBorder(Window window) {
        try {
            var currentStyle = WinApiService.GetWindowLongPtr(window.WindowHandle, GwlType.Style);
            if (WinApiService.SetWindowLongPtr(window.WindowHandle, GwlType.Style, (IntPtr) ((long) currentStyle | (long) WsStyleType.OverlappedWindow)) != IntPtr.Zero) {
                return true;
            }
            return false;
        }
        catch {
            return false;
        }
    }

    public void RedrawWindow(Window window) {
        WinApiService.RedrawMenuBar(window.WindowHandle);
    }
}

}