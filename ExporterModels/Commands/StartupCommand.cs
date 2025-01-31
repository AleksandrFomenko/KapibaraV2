using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ExporterModels.ViewModels;
using ExporterModels.Views;

namespace ExporterModels.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new ExporterModelsViewModel();
        var view = new ExporterModelsView(viewModel);
        view.ShowDialog();
    }
}