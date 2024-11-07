using System.Runtime.Serialization;
using ReactiveUI;

namespace WinResizer.ViewModels;

[DataContract]
public class SavedWindowConfig : BaseViewModel {
    private Rect _rect = new();
    [DataMember]
    public Rect Rect {
        get => _rect;
        set => this.RaiseAndSetIfChanged(ref _rect, value);
    }
    private bool _isResizable;
    [DataMember]
    public bool IsResizable {
        get => _isResizable;
        set => this.RaiseAndSetIfChanged(ref _isResizable, value);
    }

    private string? _processName;
    [DataMember]
    public string? ProcessName {
        get => _processName;
        set => this.RaiseAndSetIfChanged(ref _processName, value);
    }


    public SavedWindowConfig() { }
    public SavedWindowConfig(SystemWindow systemWindow) {
        ProcessName = systemWindow.Name;
        Rect        = systemWindow.Dimensions;
    }
}