using Autodesk.Revit.Attributes;
using KapibaraUI.Services.Appearance;
using KapibaraUI.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using SortingCategories.Model;
using SortingCategories.ViewModels;
using SortingCategories.Views;
using Wpf.Ui;
using Wpf.Ui.Abstractions;



namespace SortingCategories.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand: ExternalCommand
{ 
    public override void Execute()
    {
        var document = Context.ActiveDocument;
        var services = new ServiceCollection();
        //Window & pages
        services.AddSingleton<SortingCategoriesView>();
        services.AddSingleton<MainFamilies>();
        services.AddSingleton<SubFamilies>();
        // vm
        services.AddSingleton<SortingCategoriesViewModel>();
        services.AddSingleton<SubFamiliesViewModel>();
        //Model
        services.AddSingleton(provider =>
        {
            return new ParametersMainFamiliesModel(document);
        });
        services.AddSingleton(provider =>
        {
            return new SubFamiliesModel(document);
        });
        
        // services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();

        var serviceProvider = services.BuildServiceProvider();
        var view = serviceProvider.GetRequiredService<SortingCategoriesView>(); 
        
        view.ShowDialog();

    }
}
