using ViewByParameter.AddFilter.Models;
using ViewByParameter.AddFilter.View;
using ViewByParameter.AddFilter.ViewModels;
using ViewByParameter.Models;

namespace ViewByParameter.ViewModels;

public sealed partial class ViewByParameterViewModel : ObservableObject
{
    private readonly IViewByParameterModel _model;
    [ObservableProperty] private List<ViewOption?> _viewOptions = null!;
    [ObservableProperty] private ViewOption? _viewOption;
    [ObservableProperty] private List<FilterOption?> _filterOptions = null!;
    [ObservableProperty] private FilterOption? _filterOption;
    [ObservableProperty] private List<string?> _projectParameters = null!;
    [ObservableProperty] private string? _projectParameter;
    [ObservableProperty] private List<FilterFromProject?> _filtersFromProject = null!;
    [ObservableProperty] private bool _isCheckedAll = true;
    [ObservableProperty] private bool _isCheckedAllFilters = true;

    private Func<AddFilterView?> ShowAddFilterWindow;

    [ObservableProperty] private List<ElementsByParameter>  _elementsByParameters = null!;
    
    
    public ViewByParameterViewModel(IViewByParameterModel model, Func<AddFilterView?> showAddFilterWindow)
    {
        _model = model;
        ShowAddFilterWindow = showAddFilterWindow;
        ViewOptions = _model.GetViewOption();
        ViewOption = ViewOptions.FirstOrDefault();
        FilterOptions = _model.GetFilterOption();
        FilterOption = FilterOptions.FirstOrDefault();
        ProjectParameters = _model.GetProjectParameters();
        ProjectParameter = ProjectParameters.FirstOrDefault();
        ElementsByParameters = _model.GetElementsByParameter();
        //FiltersFromProject = _model.GetFiltersFromProject();
        
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
        FiltersFromProject = e.SelectedFilters;
    }
    

    [RelayCommand]
    private void AddFilter()
    {
        var view = ShowAddFilterWindow.Invoke();
        
        var viewModel = view?.ViewModel;

        if (viewModel == null) return;
        viewModel.FiltersChanged += OnFiltersChanged;
        view?.ShowDialog(); 
            
        viewModel.FiltersChanged -= OnFiltersChanged;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void Execute()
    {
        
    }
}