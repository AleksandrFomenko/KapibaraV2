using Autodesk.Revit.Attributes;
using KapibaraV2.ViewModels;
using KapibaraV2.Views;
using Nice3point.Revit.Toolkit.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.Commands.mvvmTest
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class KapibaraTestMvvm : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new TestViewModel();
            var view = new MVVM(viewModel);
            view.ShowDialog();
        }
    }
}
