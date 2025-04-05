using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SortingCategories.Views;

public sealed partial class SortingCategoriesView : FluentWindow
{
    public SortingCategoriesView(SortingCategoriesViewModel viewModel)
    {
        ThemeWatcherService.Initialize();
        ThemeWatcherService.Watch(this);

        ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);

        DataContext = viewModel;
        InitializeComponent();
    }
}