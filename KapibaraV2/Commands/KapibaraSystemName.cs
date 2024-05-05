using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using KapibaraV2.ViewModels;
using KapibaraV2.Views;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Kapibara2.Views.MepGeneral;

namespace KapibaraV2.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class KapibaraSystemName : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // var viewModel = new KapibaraV2ViewModel();
            //var view = new KapibaraV2View(viewModel);
            //view.ShowDialog();
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            SystemNameView wpffForm = new SystemNameView(doc);
            wpffForm.ShowDialog();
            return Result.Succeeded;


        }
    }
}