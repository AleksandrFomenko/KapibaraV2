using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using SetParametersByActiveView.ViewModels;
using SetParametersByActiveView.Views;

namespace SetParametersByActiveView.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new SetParametersByActiveViewViewModel();
        var view = new SetParametersByActiveViewView(viewModel);
        view.ShowDialog();
    }
}