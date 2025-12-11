using RiserMate.Abstractions;

namespace RiserMate.Implementation;

public class FilterCreationService : IFilterCreationService
{
    
    private readonly Document? _document = Context.ActiveDocument;
    public ParameterFilterElement CreateFilter(string nameParameter, string value)
    {
        var categories = GetCategoriesByParameter(nameParameter);
        var parameterId = SearchParameter(nameParameter);
        
        var categoryIds = categories.Select(cat => new ElementId(cat)).ToList();
        var uniqueName = GetUniqueFilterName(nameParameter, value);
        var filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value, true));
        return ParameterFilterElement.Create(_document, uniqueName, categoryIds, filter);
    }
    
    private ElementId? SearchParameter(string name)
    {
        if (_document == null) return null;
        var bindingMap = _document.ParameterBindings;
        var iterator = bindingMap.ForwardIterator();
        
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
    
    private List<BuiltInCategory> GetCategoriesByParameter(string parameterName)
    {
        var bindingMap = _document?.ParameterBindings;
        var iterator = bindingMap?.ForwardIterator();
        iterator?.Reset();

        while (iterator != null && iterator.MoveNext())
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

        return null!;
    }
    private string GetUniqueFilterName(string parameterName, string baseName, int suffix = 0)
    {
        var filterCollector = new FilteredElementCollector(_document)
            .OfClass(typeof(ParameterFilterElement))
            .Cast<ParameterFilterElement>();
        
        var newName = suffix == 0 ? $"{parameterName}_{baseName}" : $"{parameterName}_{baseName}_{suffix}";
        
        var nameExists = filterCollector.Any(f => f.Name == newName);
        
        return nameExists ? GetUniqueFilterName(parameterName, baseName,suffix + 1) :
            newName;
    }
}