namespace System_name.Models.View3D;

public static class Filter
{
    public static ParameterFilterElement createFilter(List <BuiltInCategory> categories, string nameParameter, string value)
    {
        /*
        var parameterId = searchParameter(nameParameter);
        var pvp = new ParameterValueProvider(parameterId);
        var rule = new FilterStringRule(
            pvp, 
            new FilterStringContains(),
            value);

        var categoryIds = categories.Select(cat => new ElementId(cat)).ToList();
        var filter = new ElementParameterFilter(rule, true);
        
        var uniqueName = GetUniqueFilterName(value);
        return ParameterFilterElement.Create(Context.Document, uniqueName, categoryIds, filter);
        */
        var parameterId = searchParameter(nameParameter);

        List<FilterRule> filterRule = new List<FilterRule>();
        
        filterRule.Add(ParameterFilterRuleFactory.CreateNotContainsRule(parameterId, value, true));
        var categoryIds = categories.Select(cat => new ElementId(cat)).ToList();
        
        var uniqueName = GetUniqueFilterName(value);
        
        var filter = new ElementParameterFilter(filterRule);
        return ParameterFilterElement.Create(Context.Document, uniqueName, categoryIds, filter);
    }

    private static ElementId searchParameter(string name)
    {
        BindingMap bindingMap = Context.Document.ParameterBindings;
        DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();
        
        while (iterator.MoveNext())
        {
            var definition = iterator.Key;
            
            if (definition != null && definition.Name == name)
            {
                return (definition as InternalDefinition)?.Id;
            }
        }
        return null; 
    }
    private static string GetUniqueFilterName(string baseName, int suffix = 0)
    {
        var filterCollector = new FilteredElementCollector(Context.Document)
            .OfClass(typeof(ParameterFilterElement))
            .Cast<ParameterFilterElement>();
        
        var newName = suffix == 0 ? baseName : $"{baseName}_{suffix}";
        
        var nameExists = filterCollector.Any(f => f.Name == newName);
        
        return nameExists ? GetUniqueFilterName(baseName, suffix + 1) :
            newName;
    }
    
}