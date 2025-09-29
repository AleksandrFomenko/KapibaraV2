using System.Windows;
using ExporterModels.Abstractions;
using ExporterModels.Dialogs.AddProject.View;
using ExporterModels.Dialogs.AddProject.ViewModel;
using ExporterModels.services;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ExporterModels.Dialogs.AddProject;

public static class AddProjectWindow
{
    public static AddProjectView Show(Window? owner, Action onClosed)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IInfoBarService, InfoBarService>();
        services.AddSingleton<AddProjectViewModel>();
        services.AddSingleton<AddProjectView>();

        var provider = services.BuildServiceProvider();

        var view = provider.GetService<AddProjectView>();
        var tws = provider.GetService<IThemeWatcherService>();
        view.Owner = owner;
        view.Show();
        tws?.SetConfigTheme();
        view.Closed += (_, _) => { onClosed?.Invoke(); };
        return view;
    }
}