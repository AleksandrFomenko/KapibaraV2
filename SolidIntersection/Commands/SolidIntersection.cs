using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using SolidIntersection.ViewModels;
using SolidIntersection.Views;

namespace SolidIntersection.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class SolidIntersection : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new SolidIntersectionViewModel();
        var view = new SolidIntersectionView(viewModel);
        view.ShowDialog();
    }
}