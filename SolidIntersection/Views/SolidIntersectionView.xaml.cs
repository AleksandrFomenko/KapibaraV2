using System.Windows;
using System.Windows.Media;
using KapibaraUI.Services.Appearance;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using SolidIntersection.ViewModels;

namespace SolidIntersection.Views;

public sealed partial class SolidIntersectionView
{
    public SolidIntersectionView(SolidIntersectionViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        SolidIntersectionViewModel.Close = Close;
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}