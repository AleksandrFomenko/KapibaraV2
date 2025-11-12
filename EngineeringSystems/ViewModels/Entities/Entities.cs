using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace EngineeringSystems.ViewModels.Entities;

public class Options(
    string name,
    double width,
    double height,
    GridLength firstColumnWidth,
    GridLength secondColumnWidth,
    bool flag)
{
    public string NameOpt { get; set; } = name;
    public double Width { get; } = width;
    public double Height { get; } = height;
    public GridLength FirstColumnWidth { get; } = firstColumnWidth;
    public GridLength SecondColumnWidth { get; } = secondColumnWidth;
    public bool Flag { get; set; } = flag;
}

public class SystemParameters(string name)
{
    public string Name { get; set; } = name;
}

public class FilterOption(string name, string revitApiMethodName)
{
    public string Name { get; set; } = name;
    public string RevitApiMethodName { get; set; } = revitApiMethodName;
}
public sealed class EngineeringSystem : INotifyPropertyChanged
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}