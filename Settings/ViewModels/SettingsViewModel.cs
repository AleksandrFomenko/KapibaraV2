using KapibaraUI.Services.Appearance;
using Settings.Configuration;
using Settings.Models;
using Settings.Views;
using Wpf.Ui.Appearance;

namespace Settings.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private List<Setting> _settings;
    [ObservableProperty] private Setting _setting;
    private Config Cfg { get; }
    private IThemeWatcherService themeWatcherService { get; set; }

    public SettingsViewModel(Config cfg, IThemeWatcherService tws)
    {
        var path = cfg.GetPath();
        themeWatcherService = tws;
        Cfg = KapibaraCore.Configuration.Configuration.LoadConfig<Config>(path);
        GetSettings();
    }
    
    public void SetSetting()
    {
        Setting = (Settings.FirstOrDefault(s => s.Name == Cfg.Setting.Name)
                   ?? Settings.FirstOrDefault()) ?? new Setting("Ошибка", Theme.Light);
    }
    partial void OnSettingChanged(Setting value)
    {
        var them = value.Theme == Theme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light; 
        var view = Host.GetService<SettingsView>();
        themeWatcherService.SetTheme(them, view);
        
        
        Cfg.Setting = value;
        Cfg.SaveConfig();
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