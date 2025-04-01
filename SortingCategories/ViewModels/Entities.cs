namespace SortingCategories.ViewModels;

public partial class RevitCategory : ObservableObject
{
    [ObservableProperty] private bool _isChecked;
    [ObservableProperty] private List<Category> _categories;
    [ObservableProperty] private Category _category;
    [ObservableProperty] private string _sorting;
    [ObservableProperty] private string _group;
}