using System.Windows;
using System.Windows.Threading;
using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Abstractions;


namespace SortingCategories.Views;

public sealed partial class SortingCategoriesView
{
    public SortingCategoriesView(SortingCategoriesViewModel viewModel,
        IThemeWatcherService themeWatcherService, 
        INavigationViewPageProvider serviceProvider)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        
        Loaded += async (sender, e) =>
        {
            RootNavigationView.SetPageProviderService(serviceProvider);
            themeWatcherService.Watch(this);
            await Dispatcher.Yield(DispatcherPriority.ContextIdle);
            RootNavigationView.Navigate(typeof(MainFamilies));
            themeWatcherService.SetConfigTheme();
        };
        
        InitializeComponent();
    }
    
}