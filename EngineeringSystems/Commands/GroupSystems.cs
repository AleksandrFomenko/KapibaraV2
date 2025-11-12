#nullable enable
using EngineeringSystems.Model.Abstractions;
using EngineeringSystems.Model.Mock;
using EngineeringSystems.services;
using EngineeringSystems.ViewModels;
using EngineeringSystems.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace EngineeringSystems.Commands;

public static class GroupSystems
{
    private static ServiceProvider? _serviceProvider;
    public static IServiceProvider Services => _serviceProvider 
                                               ?? throw new InvalidOperationException("Host not started");
    public static void StartMock()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<GroupSystemsView>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<GroupSystemsViewModel>();
        services.AddSingleton<IGroupSystemsModel, GroupSystemsMockModel>();
        services.AddSingleton<WindowProvider>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    public static void Start()
    {
        var tws = Services.GetRequiredService<IThemeWatcherService>();
        var view = Services.GetRequiredService<GroupSystemsView>();
        var windowProvider = Services.GetRequiredService<WindowProvider>();
        windowProvider.SetWindowOwner(view);
        
        view.Show();
        var theme = ApplicationTheme.Dark;
        var backdrop = WindowBackdropType.Tabbed;
        
        ApplicationThemeManager.Apply(theme, backdrop);
        WindowBackgroundManager.UpdateBackground(view, theme, backdrop);
    }
}