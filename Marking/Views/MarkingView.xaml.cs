using System.Windows;
using KapibaraUI.Services.Appearance;
using Marking.ViewModels;

namespace Marking.Views;

public sealed partial class MarkingView
{
    public MarkingView(MarkingViewModel viewModel, IThemeWatcherService themeService)
    {
        themeService.Watch(this);
        DataContext = viewModel;
        themeService.SetConfigTheme();
        SourceInitialized += (s, e) => themeService.SetConfigTheme();
        viewModel.MinimizeWindow += () =>  WindowState = WindowState.Minimized;
        viewModel.Model.SendMaximize += () =>  WindowState = WindowState.Normal;
        InitializeComponent();
        
    }
}