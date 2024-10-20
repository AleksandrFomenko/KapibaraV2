using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using TestLib.ViewModels;
using TestLib.Views;


namespace TestLib.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class TestLib : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new TestLibViewModel();
        var view = new TestLibView(viewModel);
        view.ShowDialog();
    }
}