using System.Windows;
using EngineeringSystems.ViewModels;
using KapibaraUI.Services.Appearance;

namespace EngineeringSystems.Views;

public partial class GroupSystemsView
{
    public GroupSystemsView(IThemeWatcherService themeWatcherService, GroupSystemsViewModel viewModel)
    {
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}