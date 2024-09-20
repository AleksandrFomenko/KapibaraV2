using Autodesk.Revit.Attributes;
using FsmModules.ViewModels;
using FsmModules.Views;
using Nice3point.Revit.Toolkit.External;

namespace FsmModules.Command;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
///
/// 
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new  FsmModulesViewModel();
        var view = new FsmModulesView(viewModel);
        view.ShowDialog();
    }
    
}