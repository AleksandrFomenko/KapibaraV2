using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using KapibaraV2.ViewModels;
using KapibaraV2.Views;
using KapibaraV2.Views.Info;
using Nice3point.Revit.Toolkit.External;

namespace KapibaraV2.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class KapibaraInfo : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new KapibaraV2ViewModel();
            var view = new KapibaraInfoView(viewModel);
            view.ShowDialog();
        }
    }
}