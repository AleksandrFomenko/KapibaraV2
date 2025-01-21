using Autodesk.Revit.Attributes;
using LevelByFloor.Models;
using Nice3point.Revit.Toolkit.External;
using LevelByFloor.ViewModels;
using LevelByFloor.Views;

namespace LevelByFloor.Commands;

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
        var model = new LevelByFloorModel(doc);
        var viewModel = new LevelByFloorViewModel(doc, model);
        var view = new LevelByFloorView(viewModel);
        view.ShowDialog();
    }
}