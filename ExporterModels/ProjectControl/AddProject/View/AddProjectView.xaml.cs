using System.Windows;
using ExporterModels.ProjectControl.AddProject.ViewModel;

namespace ExporterModels.ProjectControl.AddProject.View;

public partial class AddProjectView : Window
{
    public AddProjectView()
    {
        AddProjectVm.Close = this.Close;
        DataContext = new AddProjectVm();
        InitializeComponent();
    }
}