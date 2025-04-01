using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using SortingCategories.ViewModels;
using SortingCategories.Views;

namespace SortingCategories.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand: ExternalCommand
{ 
    public override void Execute()
    {
        var doc = Context.ActiveDocument;
        var viewModel = new SortingCategoriesViewModel(doc);
        var view = new SortingCategoriesView(viewModel);
        view.ShowDialog();
    }
    
}