using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinResizer {

/// <summary>
/// Defines a class that acts as model for active windows running on the system
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public partial class Window : ObservableObject, IComparable<Window>, IEquatable<Window> {

    /// <summary>
    /// Gets or Sets the id of the window used as identification
    /// </summary>
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string? _name = string.Empty;

    /// <summary>
    /// Gets or Sets the window handle used to manipulate window information through the WinApi
    /// </summary>
    [ObservableProperty]
    private IntPtr _windowHandle;

    /// <summary>
    /// Gets or Sets the description of the application that owns the window
    /// <remarks>Defaults to an empty string</remarks>
    /// </summary>
    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>
    /// Gets or Sets the icon of the application that owns the window
    /// </summary>
    [ObservableProperty]
    private Bitmap? _icon;

    [ObservableProperty]
    private int _iconIdx = -1;

    /// <summary>
    /// Gets or Sets the dimensions of the window
    /// </summary>
    [ObservableProperty]
    private Rect _dimensions;

    /// <summary>
    /// Gets the location of the application within the virtual space
    /// </summary>
    public string Location => $"{Dimensions.Left} x {Dimensions.Top}";

    /// <summary>
    /// Gets the resolution of the application
    /// </summary>
    public string Resolution => $"{Dimensions.Right - Dimensions.Left} x {Dimensions.Bottom - Dimensions.Top}";



    /// <summary>
    /// Compare if the current <see cref="Window"/> represents the same item as a given <see cref="Window"/>
    /// </summary>
    /// <param name="other"><see cref="Window"/> to compare against</param>
    /// <returns>Returns <see langword="true"/> if the <see cref="Window"/> represent the same <see cref="Window"/>, otherwise returns <see langword="false"/></returns>
    public int CompareTo(Window? other) {
        if (other?.Id == Id) {
            return 0;
        }
        return -1;
    }

    /// <summary>
    /// Comapares the current <see cref="Window"/> to a given <see cref="Window"/>
    /// </summary>
    /// <param name="other"><see cref="Window"/> to compare to the current instance</param>
    /// <returns>Returns <see langword="true"/> if the current istance is equal to the given <see cref="Window"/>, otherwise returns <see langword="false"/></returns>
    public bool Equals(Window? other)
        => other?.Id == Id
        && other?.WindowHandle == WindowHandle
        && other?.Name == Name
        && other?.Description == Description
        && other?.Dimensions == Dimensions;

    /// <summary>
    /// Comapares the current <see cref="Window"/> to a given <see cref="object"/>
    /// </summary>
    /// <param name="obj"><see cref="object"/> to compare to the current instance</param>
    /// <returns>Returns <see langword="true"/> if the current istance is equal to the given <see cref="object"/>, otherwise returns <see langword="false"/></returns>
    public override bool Equals(object? obj)
        => obj is Window window
        && Equals(window);

    public override int GetHashCode() {
        return HashCode.Combine(Id, Name, WindowHandle, Description, Icon, IconIdx, Dimensions);
    }
    public static bool operator ==(Window? left, Window? right) {
        return Equals(left, right);
    }
    public static bool operator !=(Window? left, Window? right) {
        return !Equals(left, right);
    }

    public override string ToString() {
        return Description;
    }
}

}