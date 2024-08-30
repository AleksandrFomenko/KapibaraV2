using Autodesk.Revit.UI;

namespace System_name.Models.View3D;

public static class View3D
{
    private static Element view = new FilteredElementCollector(Context.Document)
        .OfClass(typeof(Autodesk.Revit.DB.View3D))
        .WhereElementIsNotElementType()
        .ToElements()
        .FirstOrDefault();

    private static ElementId viewType = Context.Document.GetElement(view.GetTypeId()).Id;
    
    public static Autodesk.Revit.DB.View3D createView3D(string name)
    {
        var view = Autodesk.Revit.DB.View3D.CreateIsometric(Context.Document, viewType);
        view.Name = name;
        return view;
    }

}