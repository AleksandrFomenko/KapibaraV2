using Autodesk.Revit.DB;

namespace KapibaraCore.Solids;

public static class Solids
{
    public static Solid GetSolid(this Element element)
    {
        if (element == null) return null;

        var options = new Options
        {
            DetailLevel = ViewDetailLevel.Fine
        };
        var geometryElement = element.get_Geometry(options);
        if (geometryElement == null)
            return null;
        return ExtractSolidFromGeometry(geometryElement);
    }

    private static Solid ExtractSolidFromGeometry(GeometryElement geometryElement)
    {
        foreach (var geomObj in geometryElement)
        {
            var solid = ProcessGeometryObject(geomObj);
            if (solid != null)
                return solid;
        }
        return null;
    }

    private static Solid ProcessGeometryObject(GeometryObject geomObj)
    {
        if (geomObj is Solid solid && solid.Volume > 0)
        {
            return solid;
        }

        if (geomObj is GeometryInstance geomInstance)
        {
            var instanceGeometry = geomInstance.GetInstanceGeometry();
            if (instanceGeometry != null)
            {
                return ExtractSolidFromGeometry(instanceGeometry);
            }
        }
        return null;
    }
    
}