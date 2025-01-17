﻿using EngineeringSystems.ViewModels;

namespace EngineeringSystems.Model;

internal class Data
{
    private Document _doc;
    private const string SystemNameMissing = "Отсутствует";
    private const string SystemNameCutMissing = "Отсутствует";

    internal Data(Document doc)
    {
        _doc = doc;
    }
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
    
    internal List<EngineeringSystem> GetSystems(string filter)
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
            .Where(s => s.NameSystem?.Contains(filter) ?? false)
            .ToList();

        return filteredResult.Any() 
            ? filteredResult 
            : new List<EngineeringSystem>
            {
                new EngineeringSystem
                {
                    NameSystem = "Не найдено подходящих систем",
                    IsChecked = false,
                    CutSystemName = string.Empty,
                    SystemId = 0
                }
            };
    }
}