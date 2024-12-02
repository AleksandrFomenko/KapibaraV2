using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Nice3point.Revit.Toolkit.External;
using SolidIntersector.ViewModels;
using SolidIntersector.Views;

namespace SolidIntersector.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class SolidIntersector : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new SolidIntersectorViewModel();
        var view = new SolidIntersectorView(viewModel);
        view.ShowDialog();
    }
}