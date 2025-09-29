﻿using Axes.ViewModels;
using Axes.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;

namespace Axes.Host;

public static class Host
{
    public static void StartMock()
    {
        var services = new ServiceCollection();
        services.AddScoped<IThemeWatcherService, ThemeWatcherService>();
        services.AddScoped<AxesView>();
        services.AddScoped<AxesViewModel>();

        var sp = services.BuildServiceProvider();
        var tws = sp.GetRequiredService<IThemeWatcherService>();
        tws.Initialize();
        var view = sp.GetRequiredService<AxesView>();

        view.SourceInitialized += (s, e) => tws.SetConfigTheme();
        view.Show();
    }
}