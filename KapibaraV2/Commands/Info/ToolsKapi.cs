using Autodesk.Revit.Attributes;
using KapibaraV2.ViewModels;
using KapibaraV2.ViewModels.Tools;
using KapibaraV2.Views.Info;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.Commands.Info
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class ToolsKapi : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new ToolsKapiViewModel();
            var view = new ToolsKapiView(viewModel);
            view.ShowDialog();
        }
    }
}
