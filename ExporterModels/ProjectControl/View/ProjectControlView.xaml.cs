using System.Windows.Controls;
using ExporterModels.ProjectControl.ViewModels;

namespace ExporterModels.ProjectControl.View;

public partial class ProjectControlView : UserControl
{
    public ProjectControlView()
    {
        DataContext = new ProjectControlViewModel();
        InitializeComponent();
    }
}