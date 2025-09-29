using KapibaraUI.Services.Appearance;
using Microsoft.Extensions.DependencyInjection;
using ProjectAxes.Abstractions;
using ProjectAxes.Factories;
using ProjectAxes.Models;
using ProjectAxes.ViewModels;
using ProjectAxes.Views;

namespace ProjectAxes.Commands;

public static class Host
{
    public static void StartAxes() => Bootstrap.StartTool<ProjectAxesModel>();
    public static void StartLevels() => Bootstrap.StartTool<ProjectLevelsModel>();
    public static void StartMock() => Bootstrap.StartTool<MockModel>();
}


public static class Bootstrap
{
    public static void StartTool<TModel>()
        where TModel : class, IModel
    {
        var services = new ServiceCollection();

        services.AddScoped<IThemeWatcherService, ThemeWatcherService>();
        if (typeof(TModel) != typeof(MockModel))
        {
            var doc = Context.ActiveDocument 
                      ?? throw new InvalidOperationException("Нет активного документа Revit.");
            services.AddSingleton(doc);
        }

        services.AddTransient<TModel>();
        services.AddTransient<ProjectToolViewModel>();

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();

        services.AddScoped<ProjectAxesView>();

        var sp = services.BuildServiceProvider();

        var factory = sp.GetRequiredService<IViewModelFactory>();
        var model   = sp.GetRequiredService<TModel>();
        var vm      = factory.Create(model);

        var view = sp.GetRequiredService<ProjectAxesView>();
        view.DataContext = vm;

        var tws = sp.GetRequiredService<IThemeWatcherService>();
        view.SourceInitialized += (s, e) => tws.SetConfigTheme();

        view.Show(Context.UiApplication.MainWindowHandle);
    }
}