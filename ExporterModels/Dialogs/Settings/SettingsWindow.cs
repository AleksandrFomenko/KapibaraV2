using System.Windows;
using ExporterModels.Dialogs.Settings.Model;
using ExporterModels.Dialogs.Settings.View;
using ExporterModels.Dialogs.Settings.ViewModel;
using ExporterModels.services;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ExporterModels.Dialogs.Settings;

public static class SettingsWindow
{
    public static SettingView Show(Window? owner, Action onClosed)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<ConfigurationService>();
        services.AddSingleton<SettingsModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingView>();

        var provider = services.BuildServiceProvider();
        var vm = provider.GetService<SettingsViewModel>();
        var view = provider.GetService<SettingView>();
        var tws = provider.GetService<IThemeWatcherService>();
        if (vm != null) vm.OwnerView = view;
        if (view != null) view.Owner = owner;
       
        view.Show();
        tws.SetConfigTheme();
        //view.Loaded += (_, __) => tws.SetConfigTheme(view);

        view.Closed += (_, _) => { onClosed?.Invoke(); };
        return view;
    }
}