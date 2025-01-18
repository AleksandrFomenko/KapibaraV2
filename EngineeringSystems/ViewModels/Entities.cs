using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EngineeringSystems.ViewModels;

public class Options
{
    public string NameOpt { get; set; }
    public double Width { get; }
    public double Height { get; }
    public GridLength FirstColumnWidth { get; }
    public GridLength SecondColumnWidth { get; }
    public bool Flag { get; set; }

    public Options(string name, double width, double height, GridLength firstColumnWidth, GridLength secondColumnWidth,
        bool flag)
    {
        NameOpt = name;
        Width = width;
        Height = height;
        FirstColumnWidth = firstColumnWidth;
        SecondColumnWidth = secondColumnWidth;
        Flag = flag;
    }
}

public class SystemParameters
{
    public string Name { get; set; }
    
    public SystemParameters(string name)
    {
        this.Name = name;
    }
}
public class EngineeringSystem : INotifyPropertyChanged
{
    private bool _isChecked;
    private string _nameSystem;
    private int _systemId;
    private string _cutSystemName;

    public string NameSystem
    {
        get => _nameSystem;
        set => SetField(ref _nameSystem, value);
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => SetField(ref _isChecked, value);
    }

    public int SystemId
    {
        get => _systemId;
        set => SetField(ref _systemId, value);
    }

    public string CutSystemName
    {
        get => _cutSystemName;
        set => SetField(ref _cutSystemName, value);
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}