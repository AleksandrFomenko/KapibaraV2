using ExporterModels.Dialogs.Settings.ViewModel;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Dialogs.Settings.View;

public partial class SettingView
{
    public SettingView(SettingsViewModel viewModel, IThemeWatcherService theme)
    {
        DataContext = viewModel;
        ViewModel = viewModel;
        theme.Watch(this);
        InitializeComponent();
    }

    public SettingsViewModel ViewModel { get; }
}