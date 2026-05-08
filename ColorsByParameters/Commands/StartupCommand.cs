using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ColorsByParameters.ViewModels;
using ColorsByParameters.Views;
using JetBrains.Annotations;

namespace ColorsByParameters.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new ColorsByParametersViewModel();
        var view = new ColorsByParametersView(viewModel);
        view.ShowDialog();
    }
}