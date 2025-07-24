using EngineeringSystems.ViewModels;

namespace EngineeringSystems.Model;

public class Data(Document doc) : IData
{
    private Document _doc = doc;
    private const string SystemNameMissing = "Отсутствует";
    private const string SystemNameCutMissing = "Отсутствует";

    
    private string GetCutSystemName(Element mepSystem)
    {
        var typeSystemId = mepSystem.GetTypeId();

        if (typeSystemId == ElementId.InvalidElementId)
        {
            return null;
        }

        var typeSystem = _doc.GetElement(typeSystemId);
        var par = typeSystem?.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM);
        return par?.AsString() == "" ? null : par?.AsString();
    }
    
    public List<EngineeringSystem> GetSystems(string filter)
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);
        
        var systems = new FilteredElementCollector(_doc)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .ToElements();

        var result = systems
            .Select(f => new EngineeringSystem
            {
                NameSystem = f?.Name ?? SystemNameMissing,
                IsChecked = false,
                CutSystemName = GetCutSystemName(f) ?? SystemNameCutMissing,
                SystemId = f?.Id?.IntegerValue ?? 0
            })
            .ToList();
        var filteredResult = result
            .Where(s => s.NameSystem?.ToLower().Contains(filter.ToLower()) ?? false)
            .OrderBy(s => s.NameSystem)
            .ToList();

        return filteredResult.Any() 
            ? filteredResult 
            :
            [
                new EngineeringSystem
                {
                    NameSystem = "Не найдено подходящих систем",
                    IsChecked = false,
                    CutSystemName = string.Empty,
                    SystemId = 0
                }
            ];
    }
}