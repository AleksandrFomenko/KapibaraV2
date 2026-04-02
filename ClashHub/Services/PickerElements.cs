
using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit;

namespace ClashHub.Services;

public class PickerElements : IPickerElements
{
    public void PickElement(int id)
    {
        var uiDoc = Context.UiDocument; 
        var doc = uiDoc.Document;

        var elementId = new ElementId(id);
        uiDoc.Selection.SetElementIds([elementId]);
        uiDoc.ShowElements(elementId);
    }
}