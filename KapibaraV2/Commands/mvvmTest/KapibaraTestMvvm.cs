﻿using Autodesk.Revit.Attributes;
using KapibaraV2.Commands.Models.mvvmTest;
using KapibaraV2.ViewModels;
using KapibaraV2.Views;
using Nice3point.Revit.Toolkit.External;


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
            var md = new Model();
            var viewModel = new TestViewModel(md);
            var view = new MVVM(viewModel);
            view.ShowDialog();
        }
    }
}
