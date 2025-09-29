using ActiveView.ViewModels;
using KapibaraUI.Services.Appearance;
using Wpf.Ui.Appearance;

namespace ActiveView.Views;

public sealed partial class ActiveViewView
{
    public ActiveViewView(ActiveViewViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}