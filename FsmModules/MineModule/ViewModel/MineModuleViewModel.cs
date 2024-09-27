using Autodesk.Revit.UI;
using FsmModules.Model;
using Autodesk.Revit.UI.Selection;
using FsmModules.MineModule.Model.MineModel;
using Microsoft.EntityFrameworkCore;

namespace FsmModules.MineModule.ViewModel;

public partial class MineModuleViewModel : ObservableObject, IWorker
{
    private Document _doc;
    private const ObjectType objectType = ObjectType.Element;
    private const string status = "Выберите геометрию для расстановки prefab модуля";
    
    private const String filter = "prefab";

    private MineModuleModel _mineModuleModel;
    
    // Levels in project
    [ObservableProperty]
    private List<string> _levels = new List<string>();
    
    // First level to put modules
    [ObservableProperty]
    private string _firstLevel;
    
    // Second level to put modules
    [ObservableProperty]
    private string _secondLevel;
    
    // Full prefab families in project
    [ObservableProperty]
    private List<string> _prefabFamilies = new List<string>();
    
    // First prefab family
    [ObservableProperty]
    private string _firstPrefabFamilies;
    
    // Second prefab family
    [ObservableProperty]
    private string _secondPrefabFamilies;
    
    // Second prefab family
    [ObservableProperty]
    private string _transferPrefabFamilies;
    

    internal MineModuleViewModel(Document doc)
    {
        _doc = doc;
        LoadLevels();
        LoadLFamilies();
        _mineModuleModel = new MineModuleModel(doc);
    }

    private void LoadLevels()
    {
        _levels = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .Select(l => l.Name)
            .ToList();
    }
    
    private void LoadLFamilies()
    {
        _prefabFamilies = new FilteredElementCollector(_doc)
            .WhereElementIsElementType()
            .Where(f => f.Name.ToLower().Contains(filter))
            .Select(l => l.Name)
            .ToList();
    }
    private bool CheckNull(object value, string errorMessage)
    {
        if (value == null)
        {
            TaskDialog.Show("Err", errorMessage);
            return true;
        }
        return false;
    }
    public void Start()
    {
        if (CheckNull(_firstLevel, "Первый уровень не выбран")) return;
        if (CheckNull(_secondLevel, "Второй уровень не выбран")) return;
        if (CheckNull(_firstPrefabFamilies, "Первый тип модуля не выбран")) return;
        if (CheckNull(_secondPrefabFamilies, "Второй тип модуля не выбран")) return;
        if (CheckNull(_transferPrefabFamilies, "Тип перехода не выбран")) return;

        var uidoc = new UIDocument(_doc);
        var sel = uidoc.Selection;
        var selectedReference = sel.PickObjects(objectType, status);
        var t = new Transaction(_doc, "Set prefab modules");
        t.Start();
        foreach (var selectedRef in selectedReference)
        {
            var selectedElement = _doc.GetElement(selectedRef);
            _mineModuleModel.Execute(selectedElement, _firstPrefabFamilies,_secondPrefabFamilies,
                _firstLevel, _secondLevel,  _transferPrefabFamilies);
        }
        t.Commit();
    }
}
