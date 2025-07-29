using KapibaraUI.Services.Appearance;
using ViewByParameter.ViewModels;

namespace ViewByParameter.Views;

public sealed partial class ViewByParameterView
{
    public ViewByParameterView(ViewByParameterViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}