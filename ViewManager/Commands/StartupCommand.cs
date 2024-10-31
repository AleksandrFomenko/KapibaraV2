using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ViewManager.ViewModels;
using ViewManager.Views;

namespace ViewManager.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new ViewManagerViewModel();
        var view = new ViewManagerView(viewModel);
        view.ShowDialog();
    }
}