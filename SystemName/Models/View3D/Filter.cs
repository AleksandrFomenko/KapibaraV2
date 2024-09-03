namespace System_name.Models.View3D;

public static class Filter
{
    public static ParameterFilterElement createFilter(List <BuiltInCategory> categories, string nameParameter, string value)
    {
        var parameterId = searchParameter(nameParameter);
        var pvp = new ParameterValueProvider(parameterId);
        var rule = new FilterStringRule(
            pvp, 
            new FilterStringContains(),
            value);

        var categoryIds = categories.Select(cat => new ElementId(cat)).ToList();
        var filter = new ElementParameterFilter(rule, true);
        return ParameterFilterElement.Create(Context.Document, value, categoryIds, filter);
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
    
}