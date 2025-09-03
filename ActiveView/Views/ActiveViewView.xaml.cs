using ActiveView.ViewModels;
using KapibaraUI.Services.Appearance;
using Wpf.Ui.Appearance;

namespace ActiveView.Views;

public sealed partial class ActiveViewView
{
    public ActiveViewView(ActiveViewViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        ActiveViewViewModel.Close = Close;
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}