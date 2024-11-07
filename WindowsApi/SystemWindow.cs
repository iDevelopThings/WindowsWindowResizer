using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using WinResizer.ViewModels;

namespace WinResizer;

public static class ProcessExtensions {
    public static string TryGetDescriptionTitle(this Process process) {
        try {
            var desc = process.MainModule?.FileVersionInfo.FileDescription ?? string.Empty;

            return !string.IsNullOrEmpty(desc)
                ? desc
                : process.ProcessName;
        }
        catch (Win32Exception e) {
            // check if `Access Denied` error
            if (e.NativeErrorCode == 5) {
                return process.ProcessName;
            }
            throw;
        }
    }
}

/// <summary>
/// Defines a class that acts as model for active windows running on the system
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class SystemWindow : BaseViewModel, IComparable<SystemWindow>, IEquatable<SystemWindow> {
    private Process _process;

    /// <summary>
    /// Gets or Sets the id of the window used as identification
    /// </summary>
    public int Id => _process.Id;
    /// <summary>
    /// The actual process name
    /// </summary>
    public string? Name => _process.ProcessName;
    /// <summary>
    /// Gets or Sets the window handle used to manipulate window information through the WinApi
    /// </summary>
    public IntPtr Hwnd => _process.MainWindowHandle;


    // private double _windowHeight;
    // [IgnoreDataMember]
    // public double WindowHeight {
    //     get => _windowHeight;
    //     set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    // }

    /// <summary>
    /// Gets or Sets the description of the application that owns the window
    /// <remarks>Defaults to an empty string</remarks>
    /// </summary>
    public string Description {
        get {
            if (string.IsNullOrEmpty(_description)) {
                this.RaisePropertyChanging();
                _description = _process.TryGetDescriptionTitle();
                this.RaisePropertyChanged();
            }
            return _description;
        }
    }
    private string? _description = string.Empty;

    /// <summary>
    /// Gets or Sets the icon of the application that owns the window
    /// </summary>
    public Bitmap? Icon {
        get {
            if (_didTryLoadIcon)
                return _icon;

            this.RaisePropertyChanging();

            _didTryLoadIcon = true;
            _icon           = WindowService.TryGetProcessIcon(_process);

            this.RaisePropertyChanged();

            return _icon;
        }
        set {
            this.RaisePropertyChanging();
            _icon           = value;
            _didTryLoadIcon = true;
            this.RaisePropertyChanged();
        }
    }
    private bool    _didTryLoadIcon = false;
    private Bitmap? _icon           = null;

    /// <summary>
    /// Gets or Sets the dimensions of the window
    /// </summary>
    public Rect Dimensions {
        get {
            var newDimensions = WinApiService.GetWindowRect(Hwnd);
            if (newDimensions != _dimensions) {
                this.RaisePropertyChanging();
                _dimensions = newDimensions;
                this.RaisePropertyChanged();
            }
            return _dimensions;
        }
        set => this.RaiseAndSetIfChanged(ref _dimensions, value);
    }
    private Rect _dimensions;

    public bool IsSelected {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
    private bool _isSelected;

    private int _windowWidth  = -1;
    private int _windowHeight = -1;

    public int WindowWidth {
        get {
            if (_windowWidth == -1) {
                this.RaisePropertyChanging();
                _windowWidth = Dimensions.Right - Dimensions.Left;
                this.RaisePropertyChanged();
            }
            return _windowWidth;
        }
        set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
    }

    public int WindowHeight {
        get {
            if (_windowHeight == -1) {
                this.RaisePropertyChanging();
                _windowHeight = Dimensions.Bottom - Dimensions.Top;
                this.RaisePropertyChanged();
            }
            return _windowHeight;
        }
        set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    }

    public SystemWindow(Process process) {
        _process = process;
    }

    /// <summary>
    /// Compare if the current <see cref="SystemWindow"/> represents the same item as a given <see cref="SystemWindow"/>
    /// </summary>
    /// <param name="other"><see cref="SystemWindow"/> to compare against</param>
    /// <returns>Returns <see langword="true"/> if the <see cref="SystemWindow"/> represent the same <see cref="SystemWindow"/>, otherwise returns <see langword="false"/></returns>
    public int CompareTo(SystemWindow? other) => other?.Id == Id ? 0 : -1;

    /// <summary>
    /// Comapares the current <see cref="SystemWindow"/> to a given <see cref="SystemWindow"/>
    /// </summary>
    /// <param name="other"><see cref="SystemWindow"/> to compare to the current instance</param>
    /// <returns>Returns <see langword="true"/> if the current istance is equal to the given <see cref="SystemWindow"/>, otherwise returns <see langword="false"/></returns>
    public bool Equals(SystemWindow? other) => other?.Id == Id;

    /// <summary>
    /// Comapares the current <see cref="SystemWindow"/> to a given <see cref="object"/>
    /// </summary>
    /// <param name="obj"><see cref="object"/> to compare to the current instance</param>
    /// <returns>Returns <see langword="true"/> if the current istance is equal to the given <see cref="object"/>, otherwise returns <see langword="false"/></returns>
    public override bool Equals(object? obj) => obj is SystemWindow window && Equals(window);

    public override int GetHashCode() => HashCode.Combine(Id);

    public static bool operator ==(SystemWindow? left, SystemWindow? right) => Equals(left, right);
    public static bool operator !=(SystemWindow? left, SystemWindow? right) => !Equals(left, right);

    public override string ToString() => Description;

    public bool Resize() {
        var newRect = new Rect(
            Dimensions.Left,
            Dimensions.Top,
            WindowWidth,
            WindowHeight
        );

        try {
            WindowService.ResizeWindow(this, newRect);
            Dimensions   = WinApiService.GetWindowRect(Hwnd);
            WindowWidth  = Dimensions.Right - Dimensions.Left;
            WindowHeight = Dimensions.Bottom - Dimensions.Top;
            return true;
        }
        catch (Exception e) {
            Console.WriteLine($"Error resizing window: {e.Message}");
        }

        return false;
    }
}