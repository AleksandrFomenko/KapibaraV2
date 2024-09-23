namespace FsmModules.Modules;

internal abstract class ModulesBase
{
    protected Document _doc;

    internal abstract Dictionary<Wall, Curve> CreateExternalWalls(Element selectedElement, WallType wallType, Level lvl,
        double wallHeight);

    internal abstract Dictionary<Wall, Curve> CreateInternalWalls(Dictionary<Wall, Curve> dictionary, WallType wallType,
        Level lvl, double wallHeight);
    private static IEnumerable<CurveLoop> GetContours(Solid solid, Element element)
    {
        try
        {
            var bbox = element.get_BoundingBox(null);
            var minPoint = bbox.Min;
            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, minPoint);

            var analyzer = ExtrusionAnalyzer.Create(solid, plane, XYZ.BasisZ);
            var face = analyzer.GetExtrusionBase();

            return face.GetEdgesAsCurveLoops();
        }
        catch (Autodesk.Revit.Exceptions.InvalidOperationException)
        {
            return Enumerable.Empty<CurveLoop>();
        }
    }
    
    internal static IEnumerable<CurveLoop> FindContours(Element element)
    {
        return GetSolids(element).SelectMany(solid => GetContours(solid, element));
    }
    
    private static IEnumerable<Solid> GetSolids(Element element)
    {
        var options = new Options 
        { 
            ComputeReferences = true,
            IncludeNonVisibleObjects = true,
            DetailLevel = ViewDetailLevel.Fine
        };
        var geometry = element.get_Geometry(options);
    
        if (geometry == null)
            return Enumerable.Empty<Solid>();
    
        return GetSolids(geometry).Where(s => s.Volume > 0);
    }
    
    private static IEnumerable<Solid> GetSolids(IEnumerable<GeometryObject> geometryObjects)
    {
        foreach (var geomObj in geometryObjects)
        {
            if (geomObj is Solid solid && solid.Volume > 0)
            {
                yield return solid;
            }
            else if (geomObj is GeometryInstance instance)
            {
                var instanceGeometry = instance.GetInstanceGeometry();
                foreach (var instSolid in GetSolids(instanceGeometry))
                {
                    yield return instSolid;
                }
            }
            else if (geomObj is GeometryElement geomElement)
            {
                foreach (var elemSolid in GetSolids(geomElement))
                {
                    yield return elemSolid;
                }
            }
        }
    }

    private void MegreWalls(List<Wall> walls)
    {
        using var t = new Transaction(Context.ActiveDocument, "Объединение стен");
        t.Start();

        for (var i = 0; i <  walls.Count; i++)
        {
            for (var j = i + 1; j <  walls.Count; j++)
            {
                try
                {
                    JoinGeometryUtils.JoinGeometry(this._doc, walls[i], walls[j]);
                }
                catch { }
            }
        }
        t.Commit();
    }
    
}