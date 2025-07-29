using ViewByParameter.AddFilter.Models;
using ViewByParameter.Models;

namespace ViewByParameter.AddFilter.ViewModels;

public partial class AddFilterViewModel : ObservableObject
{
    private readonly IAddFilterModel _model;

    [ObservableProperty] private string _filterByName = string.Empty;
    [ObservableProperty] private List<FilterFromProject>? _filtersFromProject;
    
    public EventHandler<FilterChangedEventArgs>? FiltersChanged;

    public AddFilterViewModel(IAddFilterModel model)
    {
        _model = model;
        FiltersFromProject = _model.GetFilterProjects(string.Empty);
    }

    partial void OnFilterByNameChanged(string value)
    {
        FiltersFromProject?.Clear();
        FiltersFromProject = _model.GetFilterProjects(value);
    }
    
    [RelayCommand]
    private void ConfirmSelection()
    {
        if (FiltersFromProject == null) return;
        var selectedFilters = FiltersFromProject.Where(f => f.IsChecked).ToList();
        
        FiltersChanged?.Invoke(this, new FilterChangedEventArgs { SelectedFilters = selectedFilters });
    }
}
