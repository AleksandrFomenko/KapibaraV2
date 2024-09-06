using Autodesk.Revit.UI;

namespace System_name.Models.View3D;

public static class View3D
{
    public static Autodesk.Revit.DB.View3D createView3D(string name)
    {
        var viewType = new FilteredElementCollector(Context.Document)
            .OfClass(typeof(Autodesk.Revit.DB.View3D))
            .WhereElementIsNotElementType()
            .Select(v => Context.Document.GetElement(v.GetTypeId()))
            .FirstOrDefault(e => e != null)?.Id;
        
        var view = Autodesk.Revit.DB.View3D.CreateIsometric(Context.Document, viewType);
        
        var uniqueName = GetUniqueViewName(name);
        view.Name = uniqueName;
        
        return view;
    }
    
    private static string GetUniqueViewName(string baseName, int suffix = 0)
    {
        var viewCollector = new FilteredElementCollector(Context.Document)
            .OfClass(typeof(Autodesk.Revit.DB.View3D))
            .WhereElementIsNotElementType()
            .ToElements();

        var newName = suffix == 0 ? baseName : $"{baseName}_{suffix}";
        var nameExists = viewCollector.Any(v => v.Name == newName);
        
        return nameExists ? GetUniqueViewName(baseName, suffix + 1) :
            newName;
    }
}
