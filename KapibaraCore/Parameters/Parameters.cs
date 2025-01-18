using Autodesk.Revit.DB;

namespace KapibaraCore.Parameters;

/// <summary>
/// Статический класс для получения параметров.
/// </summary>
public static class Parameters
{ 
    /// <summary>
    /// Записывает значение в параметр.
    /// </summary>
    public static void SetParameterValue(this Parameter parameter, object value) 
    {
        if (parameter == null || parameter.IsReadOnly) return; 
        StorageType storageType = parameter.StorageType; 
        switch (storageType) 
        { 
            case StorageType.Integer: 
            { 
                if (value is int intValue) 
                { 
                    parameter.Set(intValue);
                }
                else 
                {
                    if (value is string valueStr)
                    {
                        valueStr = valueStr.ToLower();
                        if (valueStr is "да" or "yes")
                        {
                            parameter.Set(1);
                            break;
                        }
                        if (valueStr is "нет" or "no")
                        {
                            parameter.Set(0);
                            break;
                        }
                    }
                    if (int.TryParse(value?.ToString(), out int result))
                    {
                        parameter.Set(result);
                    }
                }
                break; 
            } 
            case StorageType.Double:
            {
                if (value is double dValue)
                {
                    var res = InternalUnits.Convert(parameter, dValue);
                    parameter.Set(res);
                }
                else
                {
                    if (double.TryParse(value?.ToString(), out double result))
                    {
                        result = InternalUnits.Convert(parameter, result);
                        parameter.Set(result);
                    }
                }
                break;
            } 
            case StorageType.String:
            {
                parameter.Set(value?.ToString() ?? string.Empty);
                break;
            } 
            case StorageType.ElementId:
            {
                if (value is ElementId eid)
                {
                    parameter.Set(eid);
                }
                else
                {
                    parameter.Set(ElementId.InvalidElementId);
                }
                break;
            }
        } 
    }

    /// <summary>
    /// Получает параметр из элемента.
    /// </summary>
    /// <param name="doc">Документ.</param>
    /// <param name="element">Element.</param>
    /// <param name="parameterName">Наименование параметра.</param>
    /// <returns>Параметр.</returns>
    public static Parameter GetParameterByName(this Element element, string parameterName)
    { 
        var par = element.LookupParameter(parameterName); 
        if (par != null) return par;
        var doc = element.Document; 
        var type = doc.GetElement(element.GetTypeId()); 
        par = type?.LookupParameter(parameterName); 
        return par ?? element.LookupParameter(parameterName);
    }
    /// <summary>
    /// Получает лист параметров из элемента.
    /// </summary>
    /// <param name="elem">Element.</param>
    /// <returns>Параметры элемента</returns>

    public static List<Parameter> GetParameters(Element elem)
    {
        return elem.Parameters.Cast<Parameter>().ToList();
    }
    
    /// <summary>
    /// Получает лист параметров из элемента.
    /// </summary>
    /// <param name="elem">Element.</param>
    /// <returns>Параметры элемента</returns>
    public static List<string> GetParameterFromFamily(Document doc, Element elem)
    {
        var famParameters = new List<FamilyParameter>();
        Family family = elem switch
        {
            FamilySymbol symbol => symbol.Family,
            
            FamilyInstance instance => (doc.GetElement(instance.GetTypeId()) as FamilySymbol)?.Family,
            
            _ => null
        };
        
        if (family != null)
        {
            Document familyDoc = doc.EditFamily(family);
            famParameters = familyDoc.FamilyManager.GetParameters().ToList();
        }
        return famParameters.Select(x=> x.Definition.Name).ToList();
    }
    
    /// <summary>
    /// Получает лист параметров из проекта, присвоенных определенной категории.
    /// </summary>
    /// <param name="doc">Документ.</param>
    /// <param name="cat">BuiltInCategory.</param>
    /// <returns>Параметры категории</returns>
    public static List<string> GetProjectParameters(this Document doc, BuiltInCategory cat)
    {
        var result = new List<string>();
        
        var bindingMap = doc.ParameterBindings;
        var iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            var definition = iterator.Key as Definition;
            var binding = bindingMap.get_Item(definition);

            var categories = binding switch
            {
                InstanceBinding instanceBinding => instanceBinding.Categories,
                TypeBinding typeBinding => typeBinding.Categories,
                _ => null
            };

            if (categories == null) continue;
            foreach (Category c in categories)
            {
                if (c == null || c.Id.IntegerValue != (int)cat) continue;
                result.Add(definition.Name);
                break;
            }
        }
        result.Sort();
        return result;
    }
    /// <summary>
    /// Получает лист параметров из проекта
    /// </summary>
    /// <param name="doc">Документ.</param>
    /// <returns>Параметры категории</returns>
    public static List<string> GetProjectParameters(this Document doc)
    {
        if (doc == null) return new List<string>();
        
        var bindingMap = doc.ParameterBindings;
        if (bindingMap == null)
            return new List<string>();

        var parameterNames = new List<string>();
        var iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            if (iterator.Key is Definition definition)
            {
                parameterNames.Add(definition.Name);
            }
        }
        
        parameterNames.Sort();

        return parameterNames;
    }

    public static IEnumerable<string> GetProjectParameters(this Document doc, ForgeTypeId type)
    {
        var bindingMap = doc.ParameterBindings;
        if (bindingMap == null)
            yield break;

        var iterator = bindingMap.ForwardIterator();
        var uniqueParameters = new HashSet<string>();

        while (iterator.MoveNext())
        {
            var definition = iterator.Key;
            if (definition == null)
                continue;


            var paramType = definition.GetDataType();

            if (paramType != type) continue;
            if (uniqueParameters.Add(definition.Name))
            {
                yield return definition.Name;
            }
        }
    }

    
    public static Definition GetProjectParameterDefinition(this Document doc, string parameterName)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));
    
        BindingMap bindingMap = doc.ParameterBindings;
        if (bindingMap == null) return null;
    
        var iterator = bindingMap.ForwardIterator();
        while (iterator.MoveNext())
        {
            if (iterator.Key is Definition definition)
            {
                if (definition.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return definition;
                }
            }
        }

        return null;
    }
}