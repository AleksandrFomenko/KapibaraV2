using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using ImportExcelByParameter.ViewModels;
using ImportExcelByParameter.Views;

namespace ImportExcelByParameter.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new ImportExcelByParameterViewModel(Context.ActiveDocument);
        var view = new ImportExcelByParameterView(viewModel);
        view.ShowDialog();
    }
}