using Autodesk.Revit.DB.Plumbing;
using RiserMate.Abstractions;


namespace RiserMate.Implementation;

public class LabelingService(View3D view) : ILabelingService
{
    private readonly View3D _view = view ?? throw new ArgumentNullException(nameof(view));
    private readonly Document? _document = Context.ActiveDocument;

    public void MarkHeatDevice(string marksHeatDevice)
    {
        if (_document == null) return;

        var heatDevices = new FilteredElementCollector(_document, _view.Id)
            .OfCategory(BuiltInCategory.OST_MechanicalEquipment)
            .WhereElementIsNotElementType()
            .ToElements();

        var mark = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_MechanicalEquipmentTags)
            .WhereElementIsElementType()
            .FirstOrDefault(m => m.Name == marksHeatDevice);

        if (mark == null) return;

        var diagonalDirection = GetDiagonalDirection();
        var offset = 10.0;

        foreach (var heatDevice in heatDevices)
        {
            var reference = new Reference(heatDevice);
            if (heatDevice.Location is not LocationPoint location) continue;

            var point = location.Point;
            var tagPoint = CalculateTagPoint(point, diagonalDirection, offset);

            var tag = IndependentTag.Create(
                _document,
                mark.Id,
                _view.Id,
                reference,
                true,
                TagOrientation.Horizontal,
                GetElbowPoint(heatDevice)
            );

            if (tag != null)
            {
                tag.LeaderEndCondition = LeaderEndCondition.Free;
                tag.TagHeadPosition = tagPoint;
                var leaderEnd = GetElbowPoint(heatDevice);
                if (leaderEnd != null) tag.SetLeaderEnd(reference, GetElbowPoint(heatDevice));
            }

            _document.Regenerate();

            try
            {
                CreateSpotElevation(heatDevice);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public void MarkPipe(string markPipe)
    {
        if (_document == null) return;

        var pipes = new FilteredElementCollector(_document, _view.Id)
            .OfCategory(BuiltInCategory.OST_PipeCurves)
            .WhereElementIsNotElementType()
            .Cast<Pipe>()
            .ToList();

        var mark = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_PipeTags)
            .WhereElementIsElementType()
            .FirstOrDefault(m => m.Name == markPipe);

        if (mark == null) return;

        var verticalPipes = pipes.Where(p =>
        {
            if (p.Location is not LocationCurve location) return false;
            var curve = location.Curve;
            var point1 = curve.GetEndPoint(0);
            var point2 = curve.GetEndPoint(1);
            var deltaZ = Math.Abs(point2.Z - point1.Z);
            var deltaXy = Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
            return deltaZ > deltaXy;
        }).ToList();

        var pipesBySystem = verticalPipes
            .GroupBy(p => p.MEPSystem?.Id)
            .Where(g => g.Key != null);
        
        var diagonalDirection = GetDiagonalDirection();
        var offset = 10;

        foreach (var systemGroup in pipesBySystem)
        {
            var validPipes = new List<Pipe>();
            var sortedPipes = systemGroup.OrderBy(p => GetMidPipe(p).Z).ToList();

            if (sortedPipes.Count == 0) continue;

            
            var currentMinZ = GetMidPipe(sortedPipes[0]).Z;
            var lengthIgnore = 5.0 * 304.8;

            for (int i = 0; i < sortedPipes.Count; i++)
            {
                var pipe = sortedPipes[i]; 
                
                if (i % 2 != 0) 
                {
                    continue;
                }
                

                if (pipe.Location is not LocationCurve location) continue;

                var curve = location.Curve;
                var length = curve.Length;

                if (length < 1500 / 304.8) continue;

                var pipeZ = GetMidPipe(pipe).Z;

                if (pipeZ >= currentMinZ && pipeZ < currentMinZ + lengthIgnore)
                {
                    validPipes.Add(pipe);
                }
                else if (pipeZ >= currentMinZ + lengthIgnore)
                {
                    currentMinZ = pipeZ;
                    validPipes.Add(pipe);
                }
            }

            foreach (var pipe in validPipes)
            {
                var reference = new Reference(pipe);

                var tag = IndependentTag.Create(
                    _document,
                    mark.Id,
                    _view.Id,
                    reference,
                    true,
                    TagOrientation.Horizontal,
                    GetMidPipe(pipe)
                );

                if (tag != null)
                {
                    var midPipe = GetMidPipe(pipe);
                    tag.LeaderEndCondition = LeaderEndCondition.Free;
                    tag.TagHeadPosition = CalculateTagPointForPipe(midPipe, diagonalDirection, offset);
                }
            }

            break;
        }

        _document.Regenerate();
    }

    private XYZ GetMidPipe(Pipe pipe)
    {
        if (pipe.Location is not LocationCurve location) return XYZ.Zero;

        var curve = location.Curve;

        var point1 = curve.GetEndPoint(0);
        var point2 = curve.GetEndPoint(1);

        return (point1 + point2) / 2.0;
    }

    private XYZ CalculateTagPointForPipe(XYZ point, XYZ direction, double offset)
    {
        var leftDirection = new XYZ(-direction.Y, direction.X, 0).Normalize();

        return new XYZ(
            point.X + leftDirection.X * offset,
            point.Y + leftDirection.Y * offset,
            point.Z - offset * 0.75
        );
    }
    

    private void CreateSpotElevation(Element element)
    {
        if (_document == null || element.Location is not LocationPoint) return;
        
        var diagonalDirection = GetDiagonalDirection();
        var offset = 10.0;

        var bendPoint = CalculateBendPoint(element);
        if (bendPoint == null) return;
        var endPoint = CalculateTagPoint2(bendPoint, diagonalDirection, offset);

        var referenceEdge = GetLowestEdgeReference(element);
        if (referenceEdge == null) return;

        var spotDimension = _document.Create.NewSpotElevation(
            _view,
            referenceEdge,
            GetClosestPointOnEdge(referenceEdge),
            bendPoint,
            endPoint,
            GetClosestPointOnEdge(referenceEdge),
            true
        );
        spotDimension.LeaderShoulderPosition = CorrectPointSpotElevation(GetClosestPointOnEdge(referenceEdge));
    }

    private Reference? GetLowestEdgeReference(Element element)
    {
        var options = new Options
        {
            ComputeReferences = true,
            View = _view
        };

        var geom = element.get_Geometry(options);
        if (geom == null) return null;

        List<Solid> allSolids = [];

        foreach (var obj in geom)
            if (obj is Solid s && s.Volume > 0)
            {
                allSolids.Add(s);
            }
            else if (obj is GeometryInstance instance)
            {
                var symbolGeom = instance.GetSymbolGeometry();
                foreach (var symObj in symbolGeom)
                    if (symObj is Solid ss && ss.Volume > 0)
                        allSolids.Add(ss);
            }

        if (allSolids.Count == 0) return null;

        var maxVolume = allSolids.Max(s => s.Volume);


        const double tolerance = 0.0001;
        var largestSolids = allSolids
            .Where(s => Math.Abs(s.Volume - maxVolume) < tolerance)
            .ToList();

        Edge? lowestEdge = null;
        var minZ = double.MaxValue;

        foreach (var solid in largestSolids)
        foreach (Edge? edge in solid.Edges)
        {
            var curve = edge?.AsCurve();

            var point1 = curve?.GetEndPoint(0);
            var point2 = curve?.GetEndPoint(1);

            if (point1 != null && point2 != null)
            {
                var edgeMinZ = Math.Min(point1.Z, point2.Z);

                if (!(edgeMinZ < minZ)) continue;
                minZ = edgeMinZ;
            }

            lowestEdge = edge;
        }

        return lowestEdge?.Reference;
    }

    private static XYZ CorrectPointSpotElevation(XYZ? xyz)
    {
        return xyz != null ? new XYZ(xyz.X, xyz.Y, xyz.Z - 0.1) : new XYZ();
    }


    private XYZ? GetClosestPointOnEdge(Reference edgeReference)
    {
        if (edgeReference == null || _view is null)
            return null;

        var element = _document!.GetElement(edgeReference.ElementId);
        if (element == null) return null;

        var options = new Options
        {
            ComputeReferences = true,
            View = _view
        };

        var geom = element.get_Geometry(options);
        if (geom == null) return null;

        var transform = Transform.Identity;

        foreach (var obj in geom)
        {
            if (obj is not GeometryInstance instance) continue;
            transform = instance.Transform;
            break;
        }

        var edge = element.GetGeometryObjectFromReference(edgeReference) as Edge;
        if (edge == null) return null;

        var curve = edge.AsCurve();
        if (curve == null) return null;

        var transformedCurve = curve.CreateTransformed(transform);

        var eyePosition = _view.Origin;
        var viewDirection = _view.ViewDirection.Normalize();

        var point1 = transformedCurve.GetEndPoint(0);
        var point2 = transformedCurve.GetEndPoint(1);

        var lowestZ = Math.Min(point1.Z, point2.Z);

        if (Math.Abs(point1.Z - lowestZ) < 0.0001)
        {
            if (Math.Abs(point2.Z - lowestZ) < 0.0001)
            {
                var toPoint1 = point1 - eyePosition;
                var toPoint2 = point2 - eyePosition;

                var projection1 = toPoint1.DotProduct(viewDirection);
                var projection2 = toPoint2.DotProduct(viewDirection);

                return projection1 > projection2 ? point1 : point2;
            }

            return point1;
        }

        return point2;
    }

    private XYZ GetDiagonalDirection()
    {
        var orientation = _view.GetOrientation();
        var forwardDirection = orientation.ForwardDirection;
        var upDirection = orientation.UpDirection;
        var rightDirection = forwardDirection.CrossProduct(upDirection).Normalize();
        return (rightDirection + upDirection).Normalize();
    }

    private XYZ? GetElbowPoint(Element element)
    {
        var bbox = element.get_BoundingBox(_view);

        if (bbox != null)
        {
            var center = (bbox.Min + bbox.Max) / 2.0;
            return center;
        }

        return null;
    }

    private XYZ CalculateTagPoint(XYZ point, XYZ direction, double offset)
    {
        return new XYZ(
            point.X + direction.X * offset,
            point.Y + direction.Y * offset,
            point.Z + direction.Z * offset / 2
        );
    }

    private XYZ CalculateTagPoint2(XYZ point, XYZ direction, double offset)
    {
        return new XYZ(
            point.X + direction.X * offset,
            point.Y + direction.Y * offset,
            point.Z + direction.Z * offset
        );
    }

    private XYZ? CalculateBendPoint(Element element)
    {
        if (element == null || _view == null)
            return null;

        var options = new Options();
        options.View = _view;
        options.ComputeReferences = true;

        var geomElement = element.get_Geometry(options);

        if (geomElement == null)
            return null;

        var faceCenters = new List<XYZ>();

        foreach (var geomObj in geomElement)
            if (geomObj is Solid solid && solid.Faces.Size > 0)
                foreach (Face face in solid.Faces)
                    try
                    {
                        var uvBox = face.GetBoundingBox();

                        if (uvBox != null && uvBox.Min != null && uvBox.Max != null)
                        {
                            var centerUv = new UV(
                                (uvBox.Min.U + uvBox.Max.U) / 2.0,
                                (uvBox.Min.V + uvBox.Max.V) / 2.0
                            );

                            var centerXyz = face.Evaluate(centerUv);

                            if (centerXyz != null) faceCenters.Add(centerXyz);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

        if (faceCenters.Count > 0)
        {
            double sumX = 0, sumY = 0, sumZ = 0;

            foreach (var center in faceCenters)
            {
                sumX += center.X;
                sumY += center.Y;
                sumZ += center.Z;
            }

            var overallCenter = new XYZ(
                sumX / faceCenters.Count,
                sumY / faceCenters.Count,
                sumZ / faceCenters.Count
            );

            return overallCenter;
        }

        var bbox = element.get_BoundingBox(_view);
        if (bbox != null && bbox.Min != null && bbox.Max != null) return (bbox.Min + bbox.Max) / 2.0;

        return null;
    }
}