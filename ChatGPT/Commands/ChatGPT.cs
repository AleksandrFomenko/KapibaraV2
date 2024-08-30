using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using ChatGPT.ViewModels;
using ChatGPT.Views;
using JetBrains.Annotations;

namespace ChatGPT.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ChatGpt : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new ChatGptViewModel();
        var view = new ChatGPTView(viewModel);
        view.ShowDialog();
    }
}