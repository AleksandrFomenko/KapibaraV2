using EngineeringSystems.ViewModels;
using KapibaraUI.Services.Appearance;

namespace EngineeringSystems.Views;

public sealed partial class EngineeringSystemsView
{
    public EngineeringSystemsView(EngineeringSystemsViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        EngineeringSystemsViewModel.Close = Close;
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
    }
}