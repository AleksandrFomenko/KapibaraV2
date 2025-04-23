using System.Windows.Controls;
using KapibaraUI.Services.Appearance;
using SortingCategories.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace SortingCategories.Views;

public partial class SubFamilies :  INavigableView<SubFamiliesViewModel>
{
    public SubFamilies(SubFamiliesViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public SubFamiliesViewModel ViewModel { get; }
}