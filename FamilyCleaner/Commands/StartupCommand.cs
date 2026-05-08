using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using FamilyCleaner.ViewModels;
using FamilyCleaner.Views;
using JetBrains.Annotations;

namespace FamilyCleaner.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand, IExternalCommand
{
    public override void Execute()
    {
        var viewModel = new FamilyCleanerViewModel();
        var view = new FamilyCleanerView(viewModel);
        view.ShowDialog();
    }
}