using HeatingDevices.ViewModels;
using KapibaraUI.Services.Appearance;


namespace HeatingDevices.Views;

public sealed partial class SpaceHeaterView
{
    public SpaceHeaterView(SpaceHeaterViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        DataContext = viewModel;
        InitializeComponent();
        themeWatcherService.Watch(this);
    } 
}