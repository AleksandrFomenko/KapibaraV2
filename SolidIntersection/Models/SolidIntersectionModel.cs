using Autodesk.Revit.UI;
using KapibaraCore.Parameters;
using KapibaraCore.Solids;


namespace SolidIntersection.Models;

public class SolidIntersectionModel
{
    private Document _doc;

    internal SolidIntersectionModel(Document doc)
    {
        _doc = doc;
    }

    internal List<SelectedItems> LoadedFamilies(string name)
    {
        var families = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_GenericModel)
            .WhereElementIsNotElementType()
            .ToElements()
            .Where(element => element?.Name != null && element.Name.Contains(name))
            .ToList();
        var itemsList = families
            .Select(f => new SelectedItems(f.Name,false))
            .ToList();
        
        return itemsList;
    }
    private List<Element> FindIntersectingElements(string elementName)
    {
        var element = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_GenericModel)
            .WhereElementIsNotElementType()
            .FirstOrDefault(e => e.Name.Equals(elementName));
        if (element == null) return new List<Element>();
        // Фильтр с BB
        var boundingBox = element.get_BoundingBox(null);
        var outline = new Outline(boundingBox.Min, boundingBox.Max);
        var filter = new BoundingBoxIntersectsFilter(outline);

        // Фильтр с солидом
        var solid = element.GetSolid();
        var solidFilter = new ElementIntersectsSolidFilter(solid);
        
        var elements = new FilteredElementCollector(_doc)
            .WherePasses(filter)
            .WherePasses(solidFilter)
            .WhereElementIsNotElementType()
            .ToList();
        return elements;
    }

    internal void Execute(IEnumerable<SelectedItems> selectedItems, string parameterName)
    {
        try
        {
            using (var t = new Transaction(_doc, "Solid intersection"))
            {
                t.Start();
                foreach (var selectedItem in selectedItems)
                {
                    var intersectionItems = FindIntersectingElements(selectedItem.GetName());
            
                    foreach (var elem in  intersectionItems)
                    {
                        var par = elem.GetParameterByName(parameterName);
                        par.SetParameterValue(selectedItem.Value);
                    }
                }
                t.Commit();
            }
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.ToString());
            throw;
        }
    }
    internal void Execute(IEnumerable<SelectedItems> selectedItems, string parameterName, string value)
    {
        try
        {
            using (var t = new Transaction(_doc, "Solid intersection"))
            {
                t.Start();
                foreach (var selectedItem in selectedItems)
                {
                    var intersectionItems = FindIntersectingElements(selectedItem.GetName());
            
                    foreach (var elem in  intersectionItems)
                    {
                        var par = elem.GetParameterByName(parameterName);
                        par.SetParameterValue(value);
                    }
                }
                t.Commit();
            }
        }
        catch (Exception e)
        {
            TaskDialog.Show("Error", e.ToString());
            throw;
        }
    }
}