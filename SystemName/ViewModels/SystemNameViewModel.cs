using System.Windows;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using System_name.Models.CoreModels;
using System_name.Models.GetElements;
using System_name.Models.View3D;
using SystemName.Models;
using SystemName.Models.Entitis;
using Transaction = Autodesk.Revit.DB.Transaction;
using View3D = System_name.Models.View3D.View3D;

namespace SystemName.ViewModels;

public partial class SystemNameViewModel : ObservableObject
{
    // Пользовательсие параметры
    [ObservableProperty]
    private List<string> _parameters = new();
    
    [ObservableProperty]
    private string _selectedParameter;
    
    // Имя системы или тип системы
    [ObservableProperty]
    private List<string> _systemParameter = new();
    
    [ObservableProperty]
    private string _selectedSystemParameter;

    // Системы труб или воздуховодов в проекте
    [ObservableProperty]
    private List<SelectedSystemName> _systemList= new ();
    
    [ObservableProperty]
    private SelectedSystemName _selectedSystem= new ();

    [ObservableProperty]
    private bool _isAllSystemsSelected = false;
    
    [ObservableProperty]
    private bool _createFilters = false;
    
    [ObservableProperty]
    private bool _onlyActiveView = false;
    
    [ObservableProperty]
    private bool _isSystemNameSelected;
    
    [ObservableProperty]
    private bool _isSystemCutNameSelected;

    private List <Element> _elements = new List<Element>();
    
    private const string SystemNameMissing = "Имя системы отсутствует";
    private const string SystemNameCutMissing = "Сокращение системы отсутствует";
    
    private const string SystemName = "Имя системы";
    private const string SystemNameCut = "Сокращение системы";


    public SystemNameViewModel()
    {
        LoadedCustomParameters();
        LoadedSystemParameters();
        LoadedSystem();
    }
    
    private void LoadedCustomParameters()
    {
        var bindingMap = Context.Document.ParameterBindings;
        var iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            var definition = iterator.Key;
            if (definition != null)
#if REVIT2023_OR_GREATER
            {
            ParameterType paramType = definition.StorageType;

                if (paramType == StorageType.Text ||
                    paramType == StorageType.Integer ||
                    paramType == StorageType.Number)
                {
                    Parameters.Add(definition.Name);
                }
            }
#else
            {
                ParameterType paramType = definition.ParameterType;

                if (paramType == ParameterType.Text ||
                    paramType == ParameterType.Integer ||
                    paramType == ParameterType.Number)
                {
                    Parameters.Add(definition.Name);
                }
            }
#endif
            foreach (var par in Parameters.Where(par => !Parameters.Contains(par)))
            {
                Parameters.Add(par);
            }

            if (!Parameters.Any())
            {
                Parameters = new List<string>();
            }
        }
    }

    private void LoadedSystemParameters()
    {
        _systemParameter ??= new List<string>();
        _systemParameter.Add(SystemName);
        _systemParameter.Add(SystemNameCut);
    }
    private void LoadedSystem()
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);
        var families = new FilteredElementCollector(Context.Document)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .ToElements();

        SystemList = families
            .Select(f => new SelectedSystemName
            {
                NameSystem = f?.Name ?? SystemNameMissing,
                IsChecked = false,
                CutSystemName = GetCutSystemName(f) ?? SystemNameCutMissing,
                SystemId = f?.Id?.IntegerValue ?? 0
            })
            .ToList();
    }

    private string GetCutSystemName(Element mepSystem)
    {
        var typeSystemId = mepSystem.GetTypeId();

        if (typeSystemId == ElementId.InvalidElementId)
        {
            return null;
        }

        var typeSystem = Context.Document.GetElement(typeSystemId);
        var par = typeSystem?.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM);
        return par?.AsString() == "" ? null : par?.AsString();
    }
    partial void OnSelectedSystemParameterChanged(string value)
    {
        IsSystemNameSelected = value == SystemName;
        IsSystemCutNameSelected = value == SystemNameCut;
    }


    private List<string> GetCheckedSystemNames()
    {
        if (_isAllSystemsSelected)
        {
            if (_isSystemNameSelected)
            {
                return SystemList
                    .Select(system => system.NameSystem)
                    .ToList();
            }
            
            return SystemList
                    .Select(system => system.CutSystemName)
                    .ToList();
        }

        if (_isSystemNameSelected)
        {
            return SystemList
                .Where(system => system.IsChecked)
                .Select(system => system.NameSystem)
                .ToList();
        }

        return SystemList
            .Where(system => system.IsChecked)
            .Select(system => system.CutSystemName)
            .ToList();
    }

    private void ExecuteTransaction()
    {
        _elements = _onlyActiveView ? GetElements.getElements(true) : _elements;
        _elements = _isAllSystemsSelected ? GetElements.getElements(false) : _elements;
        
        if (!_elements.Any())
        {
            _elements = GetElements.GetElementsInSystem(GetCheckedSystemNames());
        }

        using var t = new Transaction(Context.Document, "Kapibara system name");
        t.Start();
        var coreSystem = new CoreSystem();
        coreSystem.Execute(_elements, _selectedParameter, IsSystemNameSelected);

        if (_createFilters)
        {
            foreach (var systemName in GetCheckedSystemNames())
            {
                var view = View3D.createView3D(systemName);
                var filter = Filter.createFilter(GetElements.MEP_cats, _selectedParameter, systemName);
                view.AddFilter(filter.Id);
                view.SetFilterVisibility(filter.Id, false);
            }
        }
        t.Commit();
    }
    
    [RelayCommand]
    private void Execute(Window window)
    {
        try
        {
            ExecuteTransaction();
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.ToString());
        }
        
        window?.Close();
    }
}