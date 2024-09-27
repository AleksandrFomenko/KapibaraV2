namespace FsmModules.Model.solidHandler;


public class SolidHandler
{

    public IEnumerable<Solid> GetSolids(Element element)
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
    
    private IEnumerable<Solid> GetSolids(IEnumerable<GeometryObject> geometryObjects)
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
}