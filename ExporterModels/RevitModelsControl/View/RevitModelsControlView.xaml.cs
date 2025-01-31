using System.Windows.Controls;
using ExporterModels.RevitModelsControl.VM;

namespace ExporterModels.RevitModelsControl.View;

public partial class RevitModelsControlView : UserControl
{
    public RevitModelsControlView()
    {
        DataContext = new RevitModelsControlViewModel();
        InitializeComponent();
    }
}