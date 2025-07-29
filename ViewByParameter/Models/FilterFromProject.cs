namespace ViewByParameter.Models;

public partial class FilterFromProject(string name) : ObservableObject
{
    public event Action<bool>? SetAllChecked; 
    
    [ObservableProperty] public bool _isChecked;
    public string Name {get; set; } = name;
    public bool IsEnabled {get; set; } = true;
    public bool IsVisible {get; set; } = true;

    public virtual void OnSetAllChecked(bool obj)
    {
        SetAllChecked?.Invoke(obj);
    }
}