using ExporterModels.Dialogs.AddModel.ViewModel;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Dialogs.AddModel.View;

public partial class AddModelView
{
    public AddModelView(AddModelViewModel viewModel, IThemeWatcherService theme)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        theme.Watch(this);
        InitializeComponent();
    }

    public AddModelViewModel ViewModel { get; }
}