using KapibaraUI.Services.Appearance;
using Settings.Configuration;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace Settings.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private List<ApplicationTheme> _themes;
    [ObservableProperty] private ApplicationTheme _theme;

    [ObservableProperty] private List<WindowBackdropType> _backdrops;
    [ObservableProperty] private WindowBackdropType _backdrop;

    private Config Cfg { get; }
    private IThemeWatcherService ThemeWatcherService { get; }

    public SettingsViewModel(Config cfg, IThemeWatcherService tws)
    {
        Cfg = cfg;                
        ThemeWatcherService = tws;
        
        Themes =
        [
            ApplicationTheme.Dark,
            ApplicationTheme.Light,
            ApplicationTheme.HighContrast
        ];

        Backdrops =
        [
            WindowBackdropType.Mica,
            WindowBackdropType.Acrylic,
            WindowBackdropType.Tabbed
        ];
        
        var cfgTheme    = Cfg.Setting?.Theme    ?? ApplicationTheme.Light;
        var cfgBackdrop = Cfg.Setting?.Backdrop ?? WindowBackdropType.Mica;

        Theme    = Themes.Contains(cfgTheme)       ? cfgTheme    : ApplicationTheme.Light;
        Backdrop = Backdrops.Contains(cfgBackdrop) ? cfgBackdrop : WindowBackdropType.Mica;
        
        ThemeWatcherService.ApplyTheme(Theme);
    }

    partial void OnThemeChanged(ApplicationTheme value)
    {
        Cfg.Setting.Theme = value;
        Cfg.SaveConfig();
        ThemeWatcherService.ApplyTheme(value);
    }

    partial void OnBackdropChanged(WindowBackdropType value)
    {
        Cfg.Setting.Backdrop = value;
        Cfg.SaveConfig();
        ThemeWatcherService.ApplyTheme(Theme);
    }
}
