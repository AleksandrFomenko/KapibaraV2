using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using EngineeringSystems.ViewModels;
using EngineeringSystems.Views;

namespace EngineeringSystems.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var doc = Context.ActiveDocument;
        var viewModel = new EngineeringSystemsViewModel(doc);
        var view = new EngineeringSystemsView(viewModel);
        view.ShowDialog();
    }
}