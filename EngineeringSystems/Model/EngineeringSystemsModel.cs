using System.Text;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using EngineeringSystems.Model.Abstractions;
using EngineeringSystems.ViewModels;
using EngineeringSystems.ViewModels.Entities;
using KapibaraCore.Elements;
using KapibaraCore.Parameters;
using Options = EngineeringSystems.ViewModels.Entities.Options;

namespace EngineeringSystems.Model;

public class EngineeringSystemsModel(Document document) : IEngineeringSystemsModel
{
    private Document _doc = document;
 
    private  List<Element> GetElementsOnActiveView(List<BuiltInCategory> categories)
    {
        var catFilter = new ElementMulticategoryFilter(categories);
    
        var elements =  new FilteredElementCollector(_doc, _doc.ActiveView.Id)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .ToElements()
            .ToList(); 

        return elements;
    }

    private List<BuiltInCategory> GetCategoriesByParameter(string parameterName)
    {
        var bindingMap = _doc.ParameterBindings;
        var iterator = bindingMap.ForwardIterator();
        iterator.Reset();

        while (iterator.MoveNext())
        {
            var definition = iterator.Key;
            var binding = iterator.Current;

            if (definition != null && definition.Name == parameterName)
            {
                CategorySet categories = null;

                if (binding is InstanceBinding instanceBinding)
                    categories = instanceBinding.Categories;
                else if (binding is TypeBinding typeBinding)
                    categories = typeBinding.Categories;

                if (categories != null)
                {
                    var builtInCategories = new List<BuiltInCategory>();
                    var catIterator = categories.ForwardIterator();
                    catIterator.Reset();

                    while (catIterator.MoveNext())
                    {
                        var category = catIterator.Current as Category;
                        if (category == null)
                            continue;

                        var id = category.Id.IntegerValue;
                        
                        if (Enum.IsDefined(typeof(BuiltInCategory), id))
                        {
                            var bic = (BuiltInCategory)id;
                            builtInCategories.Add(bic);
                        }
                    }

                    return builtInCategories;
                }
            }
        }

        return null;
    }
    
    private string GetSystemType(Element elem)
    {
        var result = "";
        if (elem is not FamilyInstance fi) return result;
        var nameList = new List<string>();
        var mp = fi.MEPModel;
        if (mp?.ConnectorManager?.Connectors == null) return result;
        foreach (Connector connector in mp.ConnectorManager.Connectors)
        {
            if (connector.MEPSystem is not MEPSystem ms) continue;
            var msType = _doc.GetElement(ms.GetTypeId());
            if (msType != null)
            {
                nameList.Add(msType.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM).AsString());
            }
        }

        if (nameList.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var stringInfo in nameList)
            {
                sb.Append(stringInfo);
                sb.Append(",");
            }

            sb.Length--;
            result = sb.ToString();
        }

        nameList.Clear();
        return result;
    }
    private Element GetSystemByName(string systemName)
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);
        
        var mepSystem = new FilteredElementCollector(_doc)
            .WherePasses(catFilter)
            .WhereElementIsNotElementType()
            .FirstOrDefault(elem => elem.Name == systemName);
        return mepSystem;
    }
    private  Element GetSystemTypeByCutName(string systemName)
    {
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem,
            BuiltInCategory.OST_DuctSystem
        };
        
        var catFilter = new ElementMulticategoryFilter(cats);

        var mepSystemType = new FilteredElementCollector(_doc)
            .WherePasses(catFilter)
            .WhereElementIsElementType()
            .FirstOrDefault(e =>
                e.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM)?.AsString() == systemName);
        return mepSystemType;
    }
    private List<Element> GetSystemByCutName(string name)
    {
        if (name == null)  return new List<Element>();

        var mepSystemType = GetSystemTypeByCutName(name);
        if (mepSystemType == null)  return new List<Element>();
        var cats = new List<BuiltInCategory>
        {
            BuiltInCategory.OST_PipingSystem, BuiltInCategory.OST_Alignments,
            BuiltInCategory.OST_DuctSystem
        };
        var catFilter = new ElementMulticategoryFilter(cats);
        var elementIds = mepSystemType.GetDependentElements(catFilter);
        
        var elements = elementIds
            .Select(id => _doc.GetElement(id)) 
            .Where(el => el != null)  
            .ToList();

        return elements;
    }
    private static List<Element>? GetElementsInSystem(Element mepSystem)
    {
        return mepSystem switch
        {
            PipingSystem pipingSystem => pipingSystem.PipingNetwork.Cast<Element>().ToList(),
            MechanicalSystem ductSystem => ductSystem.DuctNetwork.Cast<Element>().ToList(),
            _ => null
        };
    }
    public static List<Element> GetElementsInSystems(List<Element> mepSystem)
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
    public List<Element> GetElementsInSystem(List<string> systemsName, bool sysName)
    {
        if (sysName)
        {
            return systemsName
                .Select(name => GetSystemByName(name))
                .Where(system => system != null)
                .SelectMany(system => GetElementsInSystem(system) ?? Enumerable.Empty<Element>())
                .ToList();
        }
        return systemsName
            .Select(name => GetSystemByCutName(name))
            .Where(system => system != null)
            .SelectMany(system => GetElementsInSystems(system) ?? Enumerable.Empty<Element>())
            .ToList();
    }

    private void Execute(List<Element> elements, string parametersUser, bool systemName)
    {
        var bp = systemName
            ? BuiltInParameter.RBS_SYSTEM_NAME_PARAM
            : BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM;
        
        foreach (var elem in elements)
        {
            var parameter = elem.get_Parameter(bp);
            if (parameter == null)
            {
                continue;
            }
            if (parameter.AsString() != null && parameter.AsString() != "")
            {
                if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                {
                    var par = elem.GetParameterByName(parametersUser);
                    par.SetParameterValue(parameter.AsString());
                    
                    foreach (var sub in elem.GetAllSubComponents())
                    {
                        par = sub.GetParameterByName(parametersUser);
                        par.SetParameterValue(parameter.AsString());
                    }
                }

                if (elem is not FamilyInstance)
                {
                    var par = elem.GetParameterByName(parametersUser);
                    par.SetParameterValue(parameter.AsString());
                }
            }
            else
            {
                if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                {
                    var result = GetSystemType(notSuperComponent);
                    var par = elem.GetParameterByName(parametersUser);
                    par.SetParameterValue(result);
                    foreach (var subelem in elem.GetAllSubComponents())
                    {
                        par = subelem.GetParameterByName(parametersUser); ;
                        par.SetParameterValue(result);
                    }
                }
            }
        }
    }
    public void Execute(
        List<string> systemNames,
        string parametersUser,
        bool flag1, 
        bool flag2, 
        Options options, 
        FilterOption filterOption)
    {
        var cats = GetCategoriesByParameter(parametersUser);
        
        var elements = options.Flag ?
            GetElementsOnActiveView(cats) :
            GetElementsInSystem(systemNames, flag1);
        
       
        using (var t = new Transaction(_doc, "Engineering systems"))
        {
            t.Start();
            Execute(elements, parametersUser, flag1);
            if (flag2)
            {
                var view3D = new View3D(_doc);
                var filter = new Filter(_doc);
                foreach (var sysName in systemNames)
                {
                    var view = view3D.CreateView3D(sysName);
                    var filt = filter.CreateFilter(cats, parametersUser, sysName, 
                        filterOption);
                    view.AddFilter(filt.Id);
                    view.SetFilterVisibility(filt.Id, false);
                }
            }
            t.Commit();
        }
    }

    public List<string> GetUserParameters()
    {
#if REVIT2022_OR_GREATER
    return _doc.GetProjectParameters(SpecTypeId.String.Text).ToList();
#else
        return _doc.GetProjectParameters(ParameterType.Text).ToList();
#endif
    }
}