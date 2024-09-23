namespace FsmModules.Modules.FacadeModule;

using FsmModules.Modules;

internal class FacadeModule : ModulesBase
{
    public FacadeModule(Document doc)
    {
        _doc = doc;
    }
    internal override Dictionary<Wall, Curve> CreateExternalWalls(Element selectedElement, WallType wallType, Level lvl, double wallHeight)
    {
        var walls = new Dictionary<Wall, Curve>();
        var contours = FindContours(selectedElement);
        foreach (var contour in contours)
        {
            foreach (var curve in contour)
            {
                var wall = Wall.Create(_doc, curve, wallType.Id, lvl.Id, wallHeight, 0, false, false);
                walls.Add(wall, curve);
            }
        }
        
        return walls;
    }

    internal override Dictionary<Wall, Curve> CreateInternalWalls(Dictionary<Wall, Curve> dictionary, WallType wallType, Level lvl, double wallHeight)
    {
        var walls = new Dictionary<Wall, Curve>();
        using var t = new Transaction(_doc, "Create Walls");
        t.Start();
        foreach (var wal in  dictionary)
        {
            var existingWall = wal.Key;
            var curve = wal.Value;

            var vector = existingWall.Orientation;
            var offsetDistance1 = _doc.GetElement(wal.Key.GetTypeId())
                .get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble() / 2;
            var offsetDistance2 = wallType.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble() / 2;
            var offsetDistance = offsetDistance1 + offsetDistance2;

            var translation = Transform.CreateTranslation(vector * offsetDistance);

            var translatedCurve = curve.CreateTransformed(translation);


            if (translatedCurve is not Line translatedLine) continue;
            var direction = translatedLine.Direction.Normalize();
            var newStart = translatedLine.GetEndPoint(0);// + (direction * offsetDistance1); 
            var newEnd = translatedLine.GetEndPoint(1);//- (direction * (offsetDistance1 + offsetDistance2)); 
            var curveResult = Line.CreateBound(newStart, newEnd);
            var newWall = Wall.Create(_doc, curveResult, wallType.Id, lvl.Id, wallHeight, 0, false, false);
            walls.Add(newWall, curveResult);
        }
        t.Commit();
        return walls;
    }
}