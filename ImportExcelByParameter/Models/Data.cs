namespace ImportExcelByParameter.Models;

internal class Data
{
    private Document _doc;

    internal Data(Document doc)
    {
        _doc = doc;
    }
    internal List<string> LoadCategory()
    {
        var categories = _doc.Settings.Categories;
        return categories
            .Cast<Category>()
            .Where(i => i.IsVisibleInUI)
            .Where(i => i.AllowsBoundParameters)
            .Where(i => i.CategoryType == CategoryType.Model)
            .Select(c => c.Name)
            .ToList();
    }

    internal List<string> LoadParameters(string categoryName)
    {
        if (categoryName == null) return new List<string>();
        var parameters = new HashSet<string>();
        var category = _doc.Settings.Categories
            .Cast<Category>()
            .FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        var cat = GetBuiltInCategory(category);
        var types = new FilteredElementCollector(_doc)
            .OfCategory(cat)
            .WhereElementIsElementType()
            .ToList();
        var elements = new FilteredElementCollector(_doc)
            .OfCategory(cat)
            .WhereElementIsNotElementType()
            .ToList();
        foreach (var type in types.Distinct()) 
        {
            if (type != null)
            {
                foreach (var param in KapibaraCore.Parameters.Parameters.GetParameters(type)
                             .Select(p => p.Definition.Name))
                {
                    parameters.Add(param); 
                }
            }
            var elem = elements.FirstOrDefault(e => e.Name == type.Name);
            if (elem != null)
            {
                foreach (var param in KapibaraCore.Parameters.Parameters.GetParameters(elem)
                             .Select(p => p.Definition.Name))
                {
                    parameters.Add(param); 
                }
            }
        }
        return parameters.ToList();
    }

    internal static BuiltInCategory GetBuiltInCategory(Category category)
    {
        if (category == null) return BuiltInCategory.INVALID;
        if (System.Enum.IsDefined(typeof(BuiltInCategory),
                category.Id.IntegerValue))
        {
            var builtInCategory = (BuiltInCategory)category.Id.IntegerValue;
            return builtInCategory;
        }
        return BuiltInCategory.INVALID;
    }
}