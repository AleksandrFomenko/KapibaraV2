using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RiserMate.Lookups;

public class RiserMateConfig : INotifyPropertyChanged
{
    private string _selectedUserParameter = string.Empty;
    public string SelectedUserParameter
    {
        get => _selectedUserParameter;
        set => SetField(ref _selectedUserParameter, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}