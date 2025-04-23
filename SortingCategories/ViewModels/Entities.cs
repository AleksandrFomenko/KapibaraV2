namespace SortingCategories.ViewModels;

public partial class RevitCategory : ObservableObject
{
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] private List<Category> _categories;
    [ObservableProperty] private Category _category;
    [ObservableProperty] private string _sorting;
    [ObservableProperty] private string _group;
}

public partial class Option
{
    public string Name { get; }
    public bool IsActiveView { get; }
    
    public Option(string name, bool activeView)
    {
        Name = name;
        IsActiveView = activeView;
    }
}


public class Algorithm
{
    public string Name { get; }
    private readonly Action<string, string, string> _executeDelegate;
    
    public Algorithm(string name, Action<string, string, string> execute)
    {
        Name = name;
        _executeDelegate = execute;
    }

    public void Execute(string parameterSort, string parameterGroup, string groupValue)
    {
        _executeDelegate?.Invoke(parameterSort, parameterGroup, groupValue);
    }
}