using Autodesk.Revit.UI;
using KapibaraCore.Parameters;
using KapibaraCore.Solids;

namespace SolidIntersection.Models;

public class SolidIntersectionModel(Document doc) : ISolidIntersectionModel
{
    public List<Project> LoadedProject()
    {
        var revitProjects = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_RvtLinks)
            .WhereElementIsNotElementType()
            .Select(p => new Project(p.Name, p.Id.IntegerValue))
            .ToList();
        
        revitProjects.Add(new Project("Текущий файл", 1));
    
        return revitProjects;
    }

    public List<string> LoadedParameters()
    {
        return doc.GetProjectParameters();
    }

    private RevitLinkInstance GetLink(Project project)
    {
        if (project.Id == 1) 
            return null;
    
        return new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_RvtLinks)
            .WhereElementIsNotElementType()
            .Cast<RevitLinkInstance>()
            .FirstOrDefault(link => link.Id.IntegerValue == project.Id);
    }
    public List<SelectedItems> LoadedFamilies(string name, Project project)
    {
        var link = GetLink(project);
        var document = link == null ? Context.ActiveDocument : link.GetLinkDocument();
        
        var families = new FilteredElementCollector( document)
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
    private List<XYZ> GetBoundingBoxCorners(XYZ min, XYZ max)
    {
        return
        [
            new XYZ(min.X, min.Y, min.Z),
            new XYZ(min.X, min.Y, max.Z),
            new XYZ(min.X, max.Y, min.Z),
            new XYZ(min.X, max.Y, max.Z),
            new XYZ(max.X, min.Y, min.Z),
            new XYZ(max.X, min.Y, max.Z),
            new XYZ(max.X, max.Y, min.Z),
            new XYZ(max.X, max.Y, max.Z)
        ];
    }
    
    private List<Element> FindIntersectingElements(string elementName, Project project)
    {
        var link = GetLink(project);
        var document = doc;
        Transform transform = null;
        if (link != null)
        {
            document = link.GetLinkDocument();
            transform = link.GetTransform();
        }
        var element = new FilteredElementCollector(document)
            .OfCategory(BuiltInCategory.OST_GenericModel)
            .WhereElementIsNotElementType()
            .FirstOrDefault(e => e.Name.Equals(elementName));
        if (element == null) return [];
        
        // Фильтр с BB
        var boundingBox = element.get_BoundingBox(null);
        if (boundingBox == null) return [];

        var originalMin = boundingBox.Min;
        var originalMax = boundingBox.Max;

        XYZ boundingBoxMin, boundingBoxMax;

        if (transform != null)
        {
            var corners = GetBoundingBoxCorners(originalMin, originalMax);
            var transformedCorners = corners
                .Select(p => transform.OfPoint(p))
                .ToList();

            boundingBoxMin = new XYZ(
                transformedCorners.Min(p => p.X),
                transformedCorners.Min(p => p.Y),
                transformedCorners.Min(p => p.Z)
            );
            boundingBoxMax = new XYZ(
                transformedCorners.Max(p => p.X),
                transformedCorners.Max(p => p.Y),
                transformedCorners.Max(p => p.Z)
            );
        }
        else
        {
            boundingBoxMin = originalMin;
            boundingBoxMax = originalMax;
        }

        var outline = new Outline(boundingBoxMin, boundingBoxMax);
        var filter = new BoundingBoxIntersectsFilter(outline);
        
        // Фильтр с солидом
        var solid = element.GetSolid();
        if (transform != null)
        {
            solid = SolidUtils.CreateTransformed(solid, transform);
        }
        var solidFilter = new ElementIntersectsSolidFilter(solid);
        
        var elements = new FilteredElementCollector(doc)
            .WherePasses(filter)
            .WherePasses(solidFilter)
            .WhereElementIsNotElementType()
            .ToList();
        return elements;
    }

    public void Execute(IEnumerable<SelectedItems> selectedItems, string parameterName, Project project)
    {
        try
        {
            using (var t = new Transaction(doc, "Solid intersection"))
            {
                t.Start();
                foreach (var selectedItem in selectedItems)
                {
                    var intersectionItems = FindIntersectingElements(selectedItem.GetName(), project);
            
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

    public void Execute(IEnumerable<SelectedItems> selectedItems, string parameterName, string value, Project project)
    {
        try
        {
            using (var t = new Transaction(doc, "Solid intersection"))
            {
                t.Start();
                foreach (var selectedItem in selectedItems)
                {
                    var intersectionItems = FindIntersectingElements(selectedItem.GetName(), project);
            
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