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
using Wpf.Ui.Appearance;


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
        if(document != null) services.AddSingleton(document);
        //Window & pages
        services.AddSingleton<SortingCategoriesView>();
        services.AddSingleton<MainFamilies>();
        services.AddSingleton<SubFamilies>();
        // vm
        services.AddSingleton<SortingCategoriesViewModel>();
        services.AddSingleton<SubFamiliesViewModel>();
        //Model
        services.AddSingleton<ParametersMainFamiliesModel>();
        services.AddSingleton<SubFamiliesModel>();
        // services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeWatcherService, ThemeWatcherService>();
        services.AddSingleton<INavigationViewPageProvider, PageService>();

        var serviceProvider = services.BuildServiceProvider();
        
        var tws = serviceProvider.GetRequiredService<IThemeWatcherService>();
        var view = serviceProvider.GetRequiredService<SortingCategoriesView>(); 
        var view1 = serviceProvider.GetRequiredService<MainFamilies>(); 
        var view2 = serviceProvider.GetRequiredService<SubFamilies>(); 
        
        tws.SetConfigTheme(view);
        tws.SetConfigTheme(view1);
        tws.SetConfigTheme(view2);
        view.ShowDialog();
    }
}
