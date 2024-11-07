using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinResizer;

public class WinApiService {

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-drawmenubar"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "DrawMenuBar", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DrawMenuBar(IntPtr hwnd);

    private static void ThrowLastError() {
        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }

    public static void RedrawMenuBar(IntPtr hwnd) {
        if (!DrawMenuBar(hwnd)) {
            ThrowLastError();
        }
    }

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);


    public static Rect GetWindowRect(IntPtr hwnd) {
        if (!GetWindowRect(hwnd, out var rect)) { }
        return rect;
    }

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SwpType uFlags);


    public static void SetWindowPos(IntPtr hwnd, Rect position, SwpType uFlags) {
        if (!SetWindowPos(hwnd, IntPtr.Zero, position.Left, position.Top, position.Right, position.Bottom, uFlags)) {
            ThrowLastError();
        }
    }

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlonga"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hwnd, GwlType nIndex);

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hwnd, GwlType nIndex);


    public static IntPtr GetWindowLongPtr(IntPtr hwnd, GwlType nIndex) {
        IntPtr returnValue;
        if (IntPtr.Size == 8) {
            // 64 bit system
            returnValue = GetWindowLongPtr64(hwnd, nIndex);
        } else {
            // 32 bit system
            returnValue = GetWindowLongPtr32(hwnd, nIndex);
        }

        if (returnValue == IntPtr.Zero) {
            ThrowLastError();
        }

        return returnValue;
    }

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlonga"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int SetWindowLongPtr32(IntPtr hWnd, GwlType nIndex, int dwNewLong);

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptra"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GwlType nIndex, IntPtr dwNewLong);


    public static IntPtr SetWindowLongPtr(IntPtr hwnd, GwlType nIndex, IntPtr dwNewLong) {
        IntPtr returnValue;
        if (IntPtr.Size == 8) {
            // 64 bit system
            returnValue = SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
        } else {
            // 32 bit system
            returnValue = (IntPtr) SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32());
        }

        if (returnValue == IntPtr.Zero) {
            ThrowLastError();
        }

        return returnValue;
    }

    /*
    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getmonitorinfoa"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoA", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref Monitor lpmi);

    /// <summary>
    /// Callback used by <see cref="EnumDisplayMonitors(IntPtr, IntPtr, EnumDisplayMonitorsDelegate, IntPtr)"/>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-monitorenumproc"/>
    /// </summary>
    private delegate bool EnumDisplayMonitorsDelegate(IntPtr hMonitor, IntPtr hdc, ref Rect lpRect, IntPtr dwData);

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaymonitors"/>
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "EnumDisplayMonitors", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumDisplayMonitorsDelegate lpfnEnum, IntPtr dwData);


    public List<Monitor> GetAllMonitors() {
        var monitors = new List<Monitor>();

        bool Callback(IntPtr hMonitor, IntPtr hdc, ref Rect lpRect, IntPtr dwData) {
            var monitor = new Monitor {
                Size          = 40, // We harcode the value as Marshal.SizeOf returns a invalid value as our struct contains extra data (40 = MONITORINFO , 72 = MONITORINFOEX)
                MonitorHandle = hMonitor
            };

            if (!GetMonitorInfo(hMonitor, ref monitor)) {
                ThrowLastError();
            }

            monitors.Add(monitor);
            return true;
        }

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Callback, IntPtr.Zero);

        return monitors;
    }*/
}