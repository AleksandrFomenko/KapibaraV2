using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using KapibaraV2.ViewModels.MepGeneral;
using KapibaraV2.Views.MepGeneral;
using KapibaraV2.Core;


namespace KapibaraV2.Commands.MepGeneral
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    internal class FloorFillerCommand : ExternalCommand
    {
        public override void Execute()
        {
            RevitApi.Initialize(ExternalCommandData);
            var viewModel = new FloorFillerModel();
            var view = new FloorFillerVIew(viewModel);
            view.ShowDialog();
        }
    }
}
