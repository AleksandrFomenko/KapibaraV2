using KapibaraUI.Services.Appearance;
using ProjectAxes.Abstractions;
using ProjectAxes.ViewModels;

namespace ProjectAxes.Views;

public sealed partial class ProjectAxesView
{
    public ProjectAxesView(IThemeWatcherService  themeWatcherService)
    {
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}