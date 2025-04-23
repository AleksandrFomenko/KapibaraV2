using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SortingCategories.Views;

public sealed partial class SortingCategoriesView
{
    public SortingCategoriesView(SortingCategoriesViewModel viewModel,
        IThemeWatcherService themeWatcherService, 
        INavigationViewPageProvider serviceProvider)
    {
        InitializeComponent();
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        
        Loaded += (_, __) =>
        {
            RootNavigationView.SetPageProviderService(serviceProvider);
            RootNavigationView.Navigate(typeof(MainFamilies));
        };
       
    }
}