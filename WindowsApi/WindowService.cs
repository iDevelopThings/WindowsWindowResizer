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

namespace WinResizer;

public struct WindowIconData {
    public int     ProcessId      { get; set; }
    public Bitmap? Icon           { get; set; }
    public bool    DidTryLoadIcon { get; set; }
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class WindowService {
    public static Dictionary<int, WindowIconData> ProcessIcons { get; set; } = new();

    public static IEnumerable<Process> GetProcesses() {
        return Process.GetProcesses()
           .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle))
           .Where(p => p.MainWindowHandle != IntPtr.Zero);
    }

    public static void ResizeWindow(SystemWindow systemWindow, Rect resolution) {
        var newWidth    = resolution.Right == 0 ? systemWindow.Dimensions.Right : resolution.Right;
        var newHeight   = resolution.Bottom == 0 ? systemWindow.Dimensions.Bottom : resolution.Bottom;
        var newPosition = new Rect {Left = systemWindow.Dimensions.Left, Top = systemWindow.Dimensions.Top, Right = newWidth, Bottom = newHeight};

        WinApiService.SetWindowPos(systemWindow.Hwnd, newPosition, SwpType.NoMove | SwpType.NoActive | SwpType.NoZOrder);
    }

    public static void RelocateWindow(SystemWindow systemWindow, Rect location) {
        WinApiService.SetWindowPos(systemWindow.Hwnd, location, SwpType.NoSize | SwpType.NoActive | SwpType.NoZOrder);
    }

    public static bool SetResizableBorder(SystemWindow systemWindow) {
        try {
            var currentStyle = WinApiService.GetWindowLongPtr(systemWindow.Hwnd, GwlType.Style);
            if (WinApiService.SetWindowLongPtr(systemWindow.Hwnd, GwlType.Style, (IntPtr) ((long) currentStyle | (long) WsStyleType.OverlappedWindow)) != IntPtr.Zero) {
                return true;
            }
            return false;
        }
        catch {
            return false;
        }
    }

    public void RedrawWindow(SystemWindow systemWindow) {
        WinApiService.RedrawMenuBar(systemWindow.Hwnd);
    }
    
    
    public static Bitmap? TryGetProcessIcon(Process process) {
        try {
            if (ProcessIcons.TryGetValue(process.Id, out var processIcon) && processIcon.DidTryLoadIcon) {
                return processIcon.Icon;
            }


            if (process.MainModule?.FileName == null || !File.Exists(process.MainModule.FileName)) {
                ProcessIcons[process.Id] = new WindowIconData {ProcessId = process.Id, DidTryLoadIcon = true};
                return null;
            }

            var icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
            if (icon == null) {
                return null;
            }

            var bitMap = icon.ToBitmap();

            using var memory = new MemoryStream();
            bitMap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            var bm = new Bitmap(memory);

            ProcessIcons[process.Id] = new WindowIconData {
                ProcessId      = process.Id,
                Icon           = bm,
                DidTryLoadIcon = true,
            };

            return bm;
        }
        catch (Win32Exception e) {
            Console.WriteLine($"Error getting icon for process {process.ProcessName}: {e.Message}");
            ProcessIcons[process.Id] = new WindowIconData {
                ProcessId      = process.Id,
                DidTryLoadIcon = true,
            };

            return null;
        }
    }
}