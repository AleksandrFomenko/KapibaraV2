using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace FsmModules.ViewModels;

public partial class FsmModulesViewModel : ObservableObject
{

    
    [RelayCommand]
    private void Execute()
    {
        var uidoc = new UIDocument(Context.Document);
        var sel = uidoc.Selection;
        var objectType = ObjectType.Element;
        var status = "Выбери ФСМ модуль";
        var selectedReferences= sel.PickObjects(objectType, status);
    }
}