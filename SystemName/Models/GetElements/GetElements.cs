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

    private static Element getSystemByName(string systemName)
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
        return mepSystem;
    }
    
    private static Element getSystemTypeByCutName(string systemName)
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);

        var mepSystemType = new FilteredElementCollector(Context.Document)
            .WherePasses(catFilter)
            .WhereElementIsElementType()
            .FirstOrDefault(e =>
                e.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM)?.AsString() == systemName);
        return mepSystemType;
    }

    private static List<Element> getSystemByCutName(string name)
    {
        var mepSystemType = getSystemTypeByCutName(name);
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem, BuiltInCategory.OST_Alignments,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);
        
        var elementIds = mepSystemType.GetDependentElements(catFilter);
        
        var elements = elementIds
            .Select(id => Context.Document.GetElement(id)) 
            .Where(el => el != null)  
            .ToList();

        return elements;

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
    private static List<Element> getElementsInSystems(List<Element> mepSystem)
    {
        var result = new List<Element>(400); 

        foreach (var sys in mepSystem)
        {
            switch (sys)
            {
                case PipingSystem pipingSystem:
                    result.AddRange(pipingSystem.PipingNetwork.Cast<Element>().Where(e => e != null)); 
                    break;
                
                case MechanicalSystem ductSystem:
                    result.AddRange(ductSystem.DuctNetwork.Cast<Element>().Where(e => e != null));
                    break;
            }
        }

        return result;
    }
    
    
    // если cut - сокращения, иначе имена
    public static List<Element> GetElementsInSystem(List<string> systemsName, bool cut)
    { 
        return cut
            ?  systemsName
            .SelectMany(name => getElementsInSystems(getSystemByCutName(name)))
            .ToList() 
            
            : systemsName
                .SelectMany(name => getElementsInSystem(getSystemByName(name)))
                .ToList();
        
    }
}