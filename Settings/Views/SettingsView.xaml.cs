using KapibaraUI.Services.Appearance;
using Settings.ViewModels;

namespace Settings.Views;

public sealed partial class SettingsView
{
    public SettingsView(SettingsViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        InitializeComponent();
        DataContext = viewModel;
    }
}