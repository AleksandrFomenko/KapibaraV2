namespace KapibaraCore.Parameters;

/// <summary>
/// Статический класс для получения параметров.
/// </summary>
public static class Parameters
{ 
    /// <summary>
    /// Записывает значение в параметр.
    /// </summary>
    public static void SetParameterValue(Parameter parameter, object value) 
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
                    parameter.Set(dValue);
                }
                else
                {
                    if (double.TryParse(value?.ToString(), out double result))
                    {
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
    /// <param name="elem">Element.</param>
    /// /// <param name="parameterName">Наименование параметра.</param>
    /// <returns>Параметр.</returns>
    
    public static Parameter GetParameterByName(Document doc, object elem, string parameterName)
    { 
        if (elem is Element element)
        {
            if (elem is FamilyInstance instance)
            {
                var par = element.LookupParameter(parameterName);
                if (par != null) return par;
                var type = doc.GetElement(instance.GetTypeId());
                par = type?.LookupParameter(parameterName);
                if (par != null) return par;
            }
            return element.LookupParameter(parameterName);
        }

        return null;
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
    /// Получает лист параметров из проекта, присвоенных определенной категории..
    /// </summary>
    /// <param name="doc">Документ.</param>
    /// <param name="cat">BuiltInCategory.</param>
    /// <returns>Параметры категории</returns>
    public static List<string> GetProjectParametersByCategory(Document doc, BuiltInCategory cat)
    {
        List<string> result = new List<string>();
        
        BindingMap bindingMap = doc.ParameterBindings;
        var iterator = bindingMap.ForwardIterator();

        while (iterator.MoveNext())
        {
            Definition definition = iterator.Key as Definition;
            Binding binding = bindingMap.get_Item(definition);
            
            CategorySet categories = null;

            if (binding is InstanceBinding instanceBinding)
            {
                categories = instanceBinding.Categories;
            }
            else if (binding is TypeBinding typeBinding)
            {
                categories = typeBinding.Categories;
            }
            
            if (categories != null)
            {
                foreach (Category c in categories)
                {
                    if (c != null && c.Id.IntegerValue == (int)cat)
                    {
                        result.Add(definition.Name);
                        break; 
                    }
                }
            }
        }
        return result;
    }
}