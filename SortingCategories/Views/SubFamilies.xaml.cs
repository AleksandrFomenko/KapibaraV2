using System.Windows;
using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace SortingCategories.Views;

public partial class SubFamilies :  INavigableView<SubFamiliesViewModel>
{
    public SubFamiliesViewModel ViewModel { get; }
    private readonly IThemeWatcherService _themeService;
    public SubFamilies(SubFamiliesViewModel viewModel,IThemeWatcherService themeService)
    {

        ViewModel = viewModel;
        DataContext = this;
        _themeService = themeService;
        _themeService.Watch(this);
        InitializeComponent();
    }
}