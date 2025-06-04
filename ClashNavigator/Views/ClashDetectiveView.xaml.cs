using ClashDetective.ViewModels;
using KapibaraUI.Services.Appearance;

namespace ClashDetective.Views;

public sealed partial class ClashDetectiveView
{
    public ClashDetectiveView(ClashDetectiveViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}