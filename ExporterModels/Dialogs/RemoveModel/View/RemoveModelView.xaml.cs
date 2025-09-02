using ExporterModels.Dialogs.RemoveModel.ViewModel;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Dialogs.RemoveModel.View;

public partial class RemoveModelView
{
    public RemoveModelView(RemoveModelViewModel viewModel, IThemeWatcherService theme)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        RemoveModelViewModel.CloseWindow = Close;
        theme.Watch(this);
        InitializeComponent();
    }

    public RemoveModelViewModel ViewModel { get; }
}