using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using SystemName.ViewModels;
using SystemName.Views;

namespace SystemName.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommandSystemName : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new SystemNameViewModel();
        var view = new SystemNameView(viewModel);
        view.ShowDialog();
    }
}