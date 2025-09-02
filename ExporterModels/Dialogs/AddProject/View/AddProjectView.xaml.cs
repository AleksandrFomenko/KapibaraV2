using ExporterModels.Dialogs.AddProject.ViewModel;
using KapibaraUI.Services.Appearance;

namespace ExporterModels.Dialogs.AddProject.View;

public sealed partial class AddProjectView
{
    public AddProjectView(AddProjectViewModel viewModel, IThemeWatcherService theme)
    {
        theme.Watch(this);
        ViewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    public AddProjectViewModel ViewModel { get; }
}