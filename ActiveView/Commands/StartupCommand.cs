using ActiveView.Models;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ActiveView.ViewModels;
using ActiveView.Views;
using Autodesk.Revit.DB.Events;

namespace ActiveView.Commands;

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
        var viewModel = new ActiveViewViewModel(doc);
        var view = new ActiveViewView(viewModel);
        view.ShowDialog();
    }
}