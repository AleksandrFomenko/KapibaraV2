﻿using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using WorkSetLinkFiles.ViewModels;
using WorkSetLinkFiles.Views;

namespace WorkSetLinkFiles.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Document doc = Context.ActiveDocument;
        var viewModel = new WorkSetLinkFilesViewModel(doc);
        var view = new WorkSetLinkFilesView(viewModel);
        view.ShowDialog();
    }
}