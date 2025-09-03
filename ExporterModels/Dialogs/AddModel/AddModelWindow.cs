using System.Windows;
using ExporterModels.Abstractions;
using ExporterModels.Dialogs.AddModel.Abstractions;
using ExporterModels.Dialogs.AddModel.View;
using ExporterModels.Dialogs.AddModel.ViewModel;
using ExporterModels.services;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace ExporterModels.Dialogs.AddModel;

public static class AddModelWindow
{
    public static AddModelView Show(Window? owner, Action onClosed)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAddedModel, Model.Model>();
        //services.AddSingleton<IAddedModel, ModelMock>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<IInfoBarService, InfoBarService>();
        services.AddSingleton<AddModelViewModel>();
        services.AddSingleton<AddModelView>();

        var provider = services.BuildServiceProvider();

        var view = provider.GetService<AddModelView>();
        var tws = provider.GetService<IThemeWatcherService>();
        tws.SetConfigTheme(view);
        view.Owner = owner;
        view.Show();
        tws?.SetConfigTheme(view);
        view.Closed += (_, _) => { onClosed?.Invoke(); };
        return view;
    }
}