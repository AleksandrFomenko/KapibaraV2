using System.Windows.Controls;
using KapibaraUI.Services.Appearance;
using RiserMate.ViewModels;
using Wpf.Ui.Abstractions.Controls;

namespace RiserMate.Views;

public partial class RiserCreator : INavigableView<RizerCreatorViewModel>
{
    public RizerCreatorViewModel ViewModel { get; }
    public RiserCreator(RizerCreatorViewModel viewModel, IThemeWatcherService themeService)
    {
        themeService.Watch(this);
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }
}