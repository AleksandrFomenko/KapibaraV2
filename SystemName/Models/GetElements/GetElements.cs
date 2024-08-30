using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace System_name.Models.GetElements;

public static class GetElements
{
    
    public static List<BuiltInCategory> MEP_cats = new List<BuiltInCategory>()
    {
        BuiltInCategory.OST_DuctCurves,
        BuiltInCategory.OST_FlexDuctCurves,
        BuiltInCategory.OST_DuctInsulations,
        BuiltInCategory.OST_DuctLinings,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_DuctAccessory,
        BuiltInCategory.OST_DuctTerminal,
        BuiltInCategory.OST_DuctFitting,
        BuiltInCategory.OST_PipeCurves,
        BuiltInCategory.OST_PipeInsulations,
        BuiltInCategory.OST_PipeAccessory,
        BuiltInCategory.OST_PipeFitting,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_Sprinklers,
        BuiltInCategory.OST_PlumbingFixtures,
        BuiltInCategory.OST_FlexPipeCurves
    };
    
    public static List<Element> getElements(bool IsActiveView)
    {
        var collector = IsActiveView
            ? new FilteredElementCollector(Context.Document, Context.Document.ActiveView.Id)
            : new FilteredElementCollector(Context.Document);
    
        var catFilter = new ElementMulticategoryFilter(MEP_cats);
    
        var elements = collector
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .ToElements()
            .ToList();  //

        return elements;
    }

    private static Element getSystem(string systemName)
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);
        
        var mepSystem = new FilteredElementCollector(Context.Document)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .FirstOrDefault(elem => elem.Name == systemName);
        return mepSystem ?? null;
    }

    private static List<Element> getElementsInSystem(Element mepSystem)
    {
        return mepSystem switch
        {
            PipingSystem pipingSystem => pipingSystem.PipingNetwork.Cast<Element>().ToList(),
            MechanicalSystem ductSystem => ductSystem.DuctNetwork.Cast<Element>().ToList(),
            _ => null
        };
    }

    public static List<Element> GetElementsInSystem(List<string> systemsName)
    {
        return systemsName
            .SelectMany(name => getElementsInSystem(getSystem(name)))
            .ToList();
    }
}