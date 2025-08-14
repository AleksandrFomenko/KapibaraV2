using HeatingDevices.ViewModels;
using HeatingDevices.Views;
using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui.Appearance;

namespace HeatingDevices;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IServiceProvider _serviceProvider;

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static void Start()
    {
        var services = new ServiceCollection();

        services.AddSingleton<SpaceHeaterViewModel>();
        services.AddSingleton<SpaceHeaterView>();
        if (Context.ActiveDocument != null) services.AddSingleton(Context.ActiveDocument);
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();


        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}