using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using KapibaraV2.ViewModels.MepGeneral;
using KapibaraV2.Views.MepGeneral;
using KapibaraV2.Core;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Media.Animation;

namespace KapibaraV2.Commands.MepGeneral
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]

    public class SystemNameCommand : ExternalCommand
    {
        public override void Execute()
        {
            RevitApi.Initialize(ExternalCommandData);
            var viewModel = new SystemNameViewModel();
            var view = new SystemNameView(viewModel);
            view.ShowDialog();
        }
    }
}
