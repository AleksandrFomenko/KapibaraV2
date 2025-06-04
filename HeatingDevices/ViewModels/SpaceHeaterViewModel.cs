using HeatingDevices.Views;
using KapibaraUI.Services.Appearance;
using Wpf.Ui.Appearance;

namespace HeatingDevices.ViewModels;

public sealed partial class SpaceHeaterViewModel : ObservableObject
{

    [RelayCommand]
    private void ChangeTheme()
    {
        var view = Host.GetService<SpaceHeaterView>();
        var tws = Host.GetService<IThemeWatcherService>();
        
        
        tws.SetTheme(
            ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark, view);
    }
}