using Axes.ViewModels;
using KapibaraUI.Services.Appearance;

namespace Axes.Views;

public sealed partial class AxesView
{
    public AxesView(AxesViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}