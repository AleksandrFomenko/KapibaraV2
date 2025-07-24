using KapibaraUI.Services.Appearance;
using WorkSetLinkFiles.ViewModels;

namespace WorkSetLinkFiles.Views;

public sealed partial class WorkSetLinkFilesView
{
    public WorkSetLinkFilesView(WorkSetLinkFilesViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        WorkSetLinkFilesViewModel.Close = Close;
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
}