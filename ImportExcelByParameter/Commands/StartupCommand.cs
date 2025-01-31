using System.Reflection;
using System.Windows.Shapes;
using Autodesk.Revit.Attributes;
using ImportExcelByParameter.Configuration;
using Nice3point.Revit.Toolkit.External;
using ImportExcelByParameter.ViewModels;
using ImportExcelByParameter.Views;
using Path = System.IO.Path;

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
        var cfg = new Config();
        var viewModel = new ImportExcelByParameterViewModel(Context.ActiveDocument, cfg);
        var view = new ImportExcelByParameterView(viewModel);
        view.ShowDialog();
    }
}