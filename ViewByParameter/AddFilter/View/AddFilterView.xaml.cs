using KapibaraUI.Services.Appearance;
using ViewByParameter.AddFilter.ViewModels;

namespace ViewByParameter.AddFilter.View;

public partial class AddFilterView
{
    public AddFilterViewModel ViewModel { get; set; }
    
    public AddFilterView(AddFilterViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}