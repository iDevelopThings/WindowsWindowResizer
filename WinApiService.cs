using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WinResizer {

/// <summary>
/// Defines a class that represents all possible GWl options for the Get and Set WindowLongPtr method
/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra">List of Gwl values</a>
/// </summary>
public enum GwlType {
    /// <summary>
    /// Retrieve the extended window styles
    /// </summary>
    ExStyle = -20,

    /// <summary>
    /// Retrieve a handle to the application instance
    /// </summary>
    HInstance = -6,

    /// <summary>
    /// Retrieve a handle to the parent window (if there is one)
    /// </summary>
    HwndParent = -8,

    /// <summary>
    /// Retrieve the identifier of the window
    /// </summary>
    Id = -12,

    /// <summary>
    /// Retrieves the window styles
    /// </summary>
    Style = -16,

    /// <summary>
    /// Retrieves the user data accociated with the window
    /// </summary>
    UserData = -21,

    /// <summary>
    /// Retrieves the pointer to the window procedure, or a handle representing the pointer to the window procedure
    /// </summary>
    WindProc = -4,
}

/// <summary>
/// Defines a class that represents all possible SWP type for the Set WindowPos method
/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos">List of Swp values</a>
/// </summary>
[Flags]
public enum SwpType : uint {
    /// <summary>
    /// Posts the request to the thread that own the window
    /// NOTE: this prevents the calling thread from blocking its execution while another thread processes the request
    /// </summary>
    AsyncWindowPos = 0x4000,

    /// <summary>
    /// Prevents the generation of the SyncPaint message
    /// </summary>
    DeferErase = 0x2000,

    /// <summary>
    /// Applies new frame styles set using the SetWindowLong function
    /// NOTE: Sends a WM_NCCALCSIZE message to the window even when the size doesn't change unless specified
    /// </summary>
    FrameChanged = 0x0020,

    /// <summary>
    /// Hides the window
    /// </summary>
    HideWindow = 0x0080,

    /// <summary>
    /// Does not active the window
    /// NOTE: If not specified the window will be activated and moved to either the top most of non-topmost group
    /// </summary>
    NoActive = 0x0010,

    /// <summary>
    /// Discards the entire contents of the client area
    /// NOTE: if not specified the contents of the client area are saved and copied back into the client area after the window is resized or repositioned
    /// </summary>
    NoCopyBits = 0x0100,

    /// <summary>
    /// Retains the current position
    /// NOTE: ignores the x and y parameters
    /// </summary>
    NoMove = 0x0002,

    /// <summary>
    /// Does not change the owner window's position in the Z order
    /// </summary>
    NoOwnerZOrder = 0x0200,

    /// <summary>
    /// Does not redraw changes
    /// NOTE: this includes the titlebar and scrollbar
    /// </summary>
    NoRedraw = 0x0008,

    /// <summary>
    /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message
    /// </summary>
    NoSendChanging = 0x0400,

    /// <summary>
    /// Retains the current size
    /// NOTE: ignores the cx and cy parameters
    /// </summary>
    NoSize = 0x0001,

    /// <summary>
    /// Retains the current Z order
    /// NOTE: ignores the hwndInsertAfter parameter
    /// </summary>
    NoZOrder = 0x0004,

    /// <summary>
    /// Displays the window
    /// </summary>
    ShowWindow = 0x0040,
}

/// <summary>
/// Defines a enum that represents all possible style that can be applied to a window
/// <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles">List of Gwl values</a>
/// </summary>
public enum WsStyleType : long {
    /// <summary>
    /// Window has a thin border
    /// </summary>
    Border = 0x00800000L,

    /// <summary>
    /// Window has a title bar
    /// <remarks>Includes <see cref="Border"/></remarks>
    /// </summary>
    Caption = 0x00C00000L,

    /// <summary>
    /// Window is a child window
    /// <remarks>Child window cannot have a menu bar</remarks>
    /// <remarks>Style cannot be used with <see cref="Popup"/></remarks>
    /// </summary>
    ChildWindow = 0x40000000L,

    /// <summary>
    /// Excludes the area occupied by child windows when drawing occurs within the parent window
    /// </summary>
    ClipChildren = 0x02000000L,

    /// <summary>
    /// Clips all other overlapping child windows out of the region of the child window to be updated
    /// </summary>
    ClipSiblings = 0x04000000L,

    /// <summary>
    /// Window is initially disabled
    /// </summary>
    Disabled = 0x08000000L,

    /// <summary>
    /// Window has a dialog border style
    /// <remarks>Window cannot have a title bar</remarks>
    /// </summary>
    DlgFrame = 0x00400000L,

    /// <summary>
    /// Window is the first control of a group of controls
    /// <remarks>The group consists of this first control and all controls defined after it, up to the next control with the <see cref="Group"/> style</remarks>
    /// </summary>
    Group = 0x00020000L,

    /// <summary>
    /// Window has a horizontal scrollbar
    /// </summary>
    HScroll = 0x00100000L,

    /// <summary>
    /// Window is initially minimized
    /// </summary>
    Iconic = 0x20000000L,

    /// <summary>
    /// Window is initially maximized
    /// </summary>
    Maximize = 0x01000000L,

    /// <summary>
    /// Window has a maximize button
    /// <remarks><see cref="SysMenu"/> also needs to be specified</remarks>
    /// <remarks>Cannot be combined with WS_EX_CONTEXTHELP </remarks>
    /// </summary>
    MaximizeBox = 0x00010000L,

    /// <summary>
    /// Window is initially minimized
    /// </summary>
    Minimize = 0x20000000L,

    /// <summary>
    /// Window has a minimize button
    /// <remarks><see cref="SysMenu"/> also needs to be specified</remarks>
    /// <remarks>Cannot be combined with WS_EX_CONTEXTHELP </remarks>
    /// </summary>
    MinimizeBox = 0x00020000L,

    /// <summary>
    /// Window is an overlaped window with a title bar and border
    /// </summary>
    Overlapped = 0x00000000L,

    /// <summary>
    /// Window is an overlapped window with a title bar and border
    /// </summary>
    OverlappedWindow = Overlapped | Caption | SysMenu | SizeBox | MinimizeBox | MaximizeBox,

    /// <summary>
    /// Window is a popup window
    /// <remarks>Cannot be used with <see cref="ChildWindow"/></remarks>
    /// </summary>
    Popup = 0x80000000L,

    /// <summary>
    /// Window is a popup window
    /// <remarks><see cref="Caption"/> need to be set for the window menu to be visible</remarks>
    /// </summary>
    PopupWindow = Popup | Border | SysMenu,

    /// <summary>
    /// Window has a sizing border
    /// </summary>
    SizeBox = 0x00040000L,

    /// <summary>
    /// Window has a window menu in it's title bar
    /// <remarks><see cref="Caption"/> need to be set for the window menu to be visible</remarks>
    /// </summary>
    SysMenu = 0x00080000L,

    /// <summary>
    /// Window can receive keyboard focus when the user presses the TAB key
    /// </summary>
    TabStop = 0x00010000L,

    /// <summary>
    /// Window is initially visible
    /// </summary>
    Visible = 0x10000000L,

    /// <summary>
    /// Window has a vertical scrollbar
    /// </summary>
    VScroll = 0x00200000L
}


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

}