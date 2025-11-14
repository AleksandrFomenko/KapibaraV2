using System.Collections.ObjectModel;
using EngineeringSystems.GroupDataStorage;
using EngineeringSystems.Model.Abstractions;
using EngineeringSystems.ViewModels.Entities;
using Group = EngineeringSystems.ViewModels.Entities.Group;

namespace EngineeringSystems.Model;

public class GroupSystemsModel : IGroupSystemsModel
{
    private Document _doc = Context.ActiveDocument!;
    private const string SystemNameMissing = "Отсутствует";
    private const string SystemNameCutMissing = "Отсутствует";
    public ObservableCollection<Group> GetGroupSystems()
    {
        var dto = GroupDataStorageService.Load(_doc);
        
        if (dto?.Data == null || dto.Data.Count == 0)
            return [];
        
        foreach (var group in dto.Data.Where(group => group.Systems == null))
        {
            group.Systems = [];
        }

        return new ObservableCollection<Group>(dto.Data);
    }

    public ObservableCollection<EngineeringSystem> GetProjectSystems()
    {
        var groups = GetGroupSystems();
        var usedSystemIds = new HashSet<int>(
            groups
                .Where(g => g.Systems != null)
                .SelectMany(g => g.Systems)
                .Select(s => s.SystemId)
                .Where(id => id != 0)
        );
        
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
                NameSystem   = f?.Name ?? SystemNameMissing,
                IsChecked    = false,
                CutSystemName = GetCutSystemName(f) ?? SystemNameCutMissing,
                SystemId     = f?.Id?.IntegerValue ?? 0
            })
            .Where(s => s.SystemId != 0 && !usedSystemIds.Contains(s.SystemId))
            .ToList();

        var filteredResult = result
            .OrderBy(s => s.NameSystem)
            .ToList();

        return filteredResult.Any() 
            ? new ObservableCollection<EngineeringSystem>(filteredResult) 
            :
            [
                new EngineeringSystem
                {
                    NameSystem   = "Не найдено подходящих систем",
                    IsChecked    = false,
                    CutSystemName = string.Empty,
                    SystemId     = 0
                }
            ];
    }
    
    public void AfterClose(ObservableCollection<Group>? groups)
    {
        if (groups is null) return;
            
        var dto = new GroupStorageDto
        {
            Data = groups.ToList()
        };

        using var t = new Transaction(_doc, "Сохранение групп инженерных систем");
        t.Start();
        GroupDataStorageService.Save(_doc, dto);
        t.Commit();
    }


    private string? GetCutSystemName(Element mepSystem)
    {
        var typeSystemId = mepSystem.GetTypeId();

        if (typeSystemId == ElementId.InvalidElementId) return "";
        
        var typeSystem = _doc.GetElement(typeSystemId);
        var par = typeSystem?.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM);
        return par?.AsString();
    }
}