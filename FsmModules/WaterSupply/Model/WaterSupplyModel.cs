using Autodesk.Revit.DB.Architecture;

namespace FsmModules.WaterSupply.Model;
internal class WaterSupplyModel
{
    private Document _doc;
    
    internal WaterSupplyModel(Document doc)
    {
        _doc = doc;
    }
    internal void createDirectShape(Room room, string categoryName)
    {
        var categoryId = _doc.Settings.Categories
            .Cast<Category>()
            .Where(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Id)
            .FirstOrDefault();
        var geometryElement = room.ClosedShell.ToList();
        var shape = DirectShape.CreateElement(_doc, categoryId);
        shape.SetShape(geometryElement);
    }
}