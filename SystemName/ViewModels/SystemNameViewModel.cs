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
    private List<string> parameters = new();
    
    [ObservableProperty]
    private string selectedParameter;
    
    // Имя системы или тип системы
    [ObservableProperty]
    private List<string> systemParameter = new();
    
    [ObservableProperty]
    private string selectedSystemParameter;

    // Системы труб или воздуховодов в проекте
    [ObservableProperty]
    private List<SelectedSystemName> systemList= new ();
    
    [ObservableProperty]
    private SelectedSystemName selectedSystem= new ();

    [ObservableProperty]
    private bool isAllSystemsSelected = false;
    
    [ObservableProperty]
    private bool createFilters = true;
    
    [ObservableProperty]
    private bool onlyActiveView = false;
    
    [ObservableProperty]
    private bool isSystemNameSelected;

    private List <Element> elements = new List<Element>();
    

    
    
    public SystemNameViewModel()
    {
        LoadedCustomParameters();
        LoadedSystemParameters();
        LoadedSystem();
    }
    
    private void LoadedCustomParameters()
    {
        BindingMap bindingMap = Context.Document.ParameterBindings;
        DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            Definition definition = iterator.Key;
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

                if (paramType == ParameterType.Text)
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
        if (systemParameter == null)
        {
            systemParameter = new List<string>();
        }
        systemParameter.Add("Имя системы");
        systemParameter.Add("Сокращение системы");
    }
    private void LoadedSystem()
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilt = new ElementMulticategoryFilter(cats);
        var families = new FilteredElementCollector(Context.Document)
            .WherePasses(catFilt)
            .WhereElementIsNotElementType()
            .ToElements();

        SystemList = families
            .Select(f => new SelectedSystemName
            {
                NameSystem = f?.Name ?? "Имя системы отсутствует",
                IsChecked = false
            })
            .ToList();
    }
    partial void OnSelectedSystemParameterChanged(string value)
    {
        IsSystemNameSelected = value == "Имя системы";
    }
    
    public List<string> GetCheckedSystemNames()
    {
        if (isAllSystemsSelected)
        {
            return SystemList
                .Select(system => system.NameSystem)  
                .ToList();
        } 
        
        return SystemList
            .Where(system => system.IsChecked)  
            .Select(system => system.NameSystem)  
            .ToList();
    }
    
    [RelayCommand]
    private void Execute(Window window)
    {
        elements = onlyActiveView ? GetElements.getElements(true) : elements;
        elements = isAllSystemsSelected ? GetElements.getElements(false) : elements;
        
        if (!elements.Any())
        {
            elements = GetElements.GetElementsInSystem(GetCheckedSystemNames());
        }
        
        using (Transaction t = new Transaction(Context.Document, "Kapibara system name"))
        {
            t.Start();
            var coreSystem = new CoreSystem();
            coreSystem.Execute(elements, selectedParameter, IsSystemNameSelected);

            if (createFilters)
            {
                foreach (var systemName in GetCheckedSystemNames())
                {
                    var view = View3D.createView3D(systemName);
                    var filter = Filter.createFilter(GetElements.MEP_cats, selectedParameter, systemName);
                    view.AddFilter(filter.Id);
                    view.SetFilterVisibility(filter.Id, false);
                }
            }
            t.Commit();
            
        }
        window?.Close();
    }
}