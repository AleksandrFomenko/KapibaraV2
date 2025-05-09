﻿using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using KapibaraV2.ViewModels.BIM;
using KapibaraV2.Views.BIM;
using KapibaraV2.Core;


namespace KapibaraV2.Commands.BIM
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    internal class ExportModels : ExternalCommand
    {
        public override void Execute()
        {
            RevitApi.Initialize(ExternalCommandData);
            var vm = new ExportModelsViewModel();
            var view = new ExportModelsView(vm);

            view.ShowDialog();
        }

    }
}
