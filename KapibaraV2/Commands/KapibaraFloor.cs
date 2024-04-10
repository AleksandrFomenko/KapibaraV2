using Autodesk.Revit.Attributes;
using KapibaraV2.ViewModels;
using KapibaraV2.Views;
using Nice3point.Revit.Toolkit.External;

namespace KapibaraV2.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class KapibaraFloor : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new KapibaraV2ViewModel();
            var view = new KapibaraV2View(viewModel);
            view.ShowDialog();
        }
    }
}