using KapibaraUI.Services.Appearance;
using Settings.Models;
using Settings.Views;
using Wpf.Ui.Appearance;

namespace Settings.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private List<Setting> _settings;
    [ObservableProperty] private Setting _setting;

    public SettingsViewModel()
    {
        GetSettings();
    }

    partial void OnSettingChanged(Setting value)
    {
        Console.WriteLine(1);
        var view = Host.GetService<SettingsView>();
        var tws = Host.GetService<IThemeWatcherService>();

        var them = value.Theme == Theme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        Console.WriteLine(them);
        tws.SetTheme(them, view);
    }
    private void GetSettings()
    {
        Settings =
        [
            new Setting("Темная", Theme.Dark),
            new Setting("Светлая", Theme.Light)
        ];
    }
}