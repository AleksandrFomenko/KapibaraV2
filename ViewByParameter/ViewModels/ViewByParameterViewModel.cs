using ViewByParameter.AddFilter.Models;
using ViewByParameter.AddFilter.View;
using ViewByParameter.AddFilter.ViewModels;
using ViewByParameter.Models;

namespace ViewByParameter.ViewModels;

public sealed partial class ViewByParameterViewModel : ObservableObject
{
    [ObservableProperty] private List<ViewOption?> _viewOptions = null!;
    [ObservableProperty] private ViewOption? _viewOption;
    [ObservableProperty] private List<FilterOption?> _filterOptions = null!;
    [ObservableProperty] private FilterOption? _filterOption;
    [ObservableProperty] private List<string?> _projectParameters = null!;
    [ObservableProperty] private string? _projectParameter;
    [ObservableProperty] private List<FilterFromProject> _filtersFromProject = [];
    [ObservableProperty] private FilterFromProject _filterFromProject = null!;
    [ObservableProperty] private bool _isCheckedAll = true;
    [ObservableProperty] private bool _isCheckedAllFilters = true;

    private readonly Func<AddFilterView?> _showAddFilterWindow;

    [ObservableProperty] private List<ElementsByParameter>  _elementsByParameters = null!;
    
    
    public ViewByParameterViewModel(IViewByParameterModel model, Func<AddFilterView?> showAddFilterWindow)
    {
        _showAddFilterWindow = showAddFilterWindow;
        ViewOptions = model.GetViewOption();
        ViewOption = ViewOptions.FirstOrDefault();
        FilterOptions = model.GetFilterOption();
        FilterOption = FilterOptions.FirstOrDefault();
        ProjectParameters = model.GetProjectParameters();
        ProjectParameter = ProjectParameters.FirstOrDefault();
        ElementsByParameters = model.GetElementsByParameter();
        
        SubscribeToElementsByParameterChanges();
    }
    private void SubscribeToElementsByParameterChanges()
    {
        foreach (var elementsByParameter in ElementsByParameters)
        {
            elementsByParameter.SetCheck += value => elementsByParameter.IsChecked = value;
            elementsByParameter.CheckButton += () => ExecuteCommand.NotifyCanExecuteChanged();
        }
    }
    
    partial void OnIsCheckedAllChanged(bool value)
    {
        foreach (var elementsByParameter in ElementsByParameters)
        {
            elementsByParameter.OnSetCheck(value);
        }
    }
    private bool CanExecute()
    {
        return ElementsByParameters.Any(p => p is { IsChecked: true }) && ProjectParameter != null;
    }
    
    private void OnFiltersChanged(object sender, FilterChangedEventArgs e)
    {
        var newFilters = e.SelectedFilters
            .Where(f => !FiltersFromProject.Any(x => x.Name == f.Name))
            .Select(f =>
            {
                f.IsVisible = false;
                return f;
            });

        FiltersFromProject = FiltersFromProject
            .Concat(newFilters)
            .ToList();
    }

    
    [RelayCommand]
    private void AddFilter()
    {
        var view = _showAddFilterWindow.Invoke();
        
        var viewModel = view?.ViewModel;

        if (viewModel == null) return;
        viewModel.FiltersChanged += OnFiltersChanged;
        view?.ShowDialog(); 
            
        viewModel.FiltersChanged -= OnFiltersChanged;
    }
    
    [RelayCommand]
    private void DeleteFilter()
    {
        if (FilterFromProject == null) return;
        FiltersFromProject = FiltersFromProject
            .Where(f => f.Name != FilterFromProject.Name)
            .ToList();
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void Execute()
    {
        
    }
}