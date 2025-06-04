using HeatingDevices.ViewModels;
using KapibaraUI.Services.Appearance;


namespace HeatingDevices.Views;

public sealed partial class SpaceHeaterView
{
    public SpaceHeaterView(SpaceHeaterViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}