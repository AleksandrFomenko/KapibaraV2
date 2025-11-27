using Autodesk.Revit.DB.Plumbing;
using KapibaraCore.Elements;
using KapibaraCore.Parameters;

namespace RiserMate.Core;

public static class RiserMateCore
{
    private static readonly Document? Document = Context.ActiveDocument;

    public static List<Pipe> GetBottomPipesByRiser(string riserName, string parameterName)
    {
        var pipes = new FilteredElementCollector(Document)
            .OfCategory(BuiltInCategory.OST_PipeCurves)
            .WhereElementIsNotElementType()
            .Cast<Pipe>()
            .Where(pipe => pipe.LookupParameter(parameterName)?.AsString() == riserName);

        var pipeHeights = new List<(Pipe Pipe, double MinZ)>();

        foreach (var pipe in pipes)
        {
            var connectors = pipe.ConnectorManager?.Connectors;
            if (connectors == null)
                continue;

            var minZ = double.PositiveInfinity;

            foreach (Connector connector in connectors)
            {
                var z = connector.Origin.Z;
                if (z < minZ)
                    minZ = z;
            }

            if (minZ < double.PositiveInfinity)
                pipeHeights.Add((pipe, minZ));
        }

        return pipeHeights
            .OrderBy(t => t.MinZ)
            .Take(2)
            .Select(t => t.Pipe)
            .ToList();
    }

    private static Connector? GetTopConnector(Element pipe)
    {
        if (pipe is not Pipe revitPipe)
            return null;

        var connectors = revitPipe.ConnectorManager?.Connectors;
        if (connectors == null)
            return null;

        Connector? topConnector = null;
        var maxZ = double.NegativeInfinity;

        foreach (Connector connector in connectors)
        {
            var cs = connector.CoordinateSystem;
            if (cs == null)
                continue;

            var z = cs.Origin.Z;
            if (z > maxZ)
            {
                maxZ = z;
                topConnector = connector;
            }
        }

        return topConnector;
    }

    private static IList<Element> GetElementsFromTopConnector(
        Pipe startPipe,
        HashSet<ElementId> visited,
        ElementId? stopId)
    {
        var cm = startPipe.ConnectorManager;
        var connectors = cm?.Connectors;
        if (connectors == null || connectors.Size == 0)
            return new List<Element>();

        var result = new List<Element>();

        if (visited.Add(startPipe.Id))
            result.Add(startPipe);

        Connector? bottomConnector = null;
        var minZ = double.PositiveInfinity;
        foreach (Connector c in connectors)
        {
            var z = c.Origin.Z;
            if (z < minZ)
            {
                minZ = z;
                bottomConnector = c;
            }
        }
        
        foreach (Connector c in connectors)
        {

            if (bottomConnector != null && c == bottomConnector)
            {
                continue;
            }
            

            TraverseFromConnector(c, visited, result, stopId);
        }

        return result;
    }

    private static void TraverseFromConnector(
        Connector connector,
        HashSet<ElementId> visited,
        IList<Element> result,
        ElementId? stopId)
    {
        foreach (Connector refConnector in connector.AllRefs)
        {
            var owner = refConnector.Owner;
            if (owner == null)
                continue;

            if (owner is MEPSystem)
                continue;

            if (owner.Id == stopId)
            {
                if (visited.Add(owner.Id))
                    result.Add(owner);
                continue;
            }

            if (!visited.Add(owner.Id))
                continue;

            result.Add(owner);

            foreach (var ownerConnector in GetAllConnectors(owner))
                TraverseFromConnector(ownerConnector, visited, result, stopId);
        }
    }


    private static IEnumerable<Connector> GetAllConnectors(Element element)
    {
        switch (element)
        {
            case MEPCurve mepCurve:
            {
                var cm = mepCurve.ConnectorManager;
                if (cm != null && cm.Connectors != null)
                    foreach (Connector c in cm.Connectors)
                        yield return c;
                break;
            }
            case FamilyInstance fi:
            {
                var mepModel = fi.MEPModel;
                var cm = mepModel?.ConnectorManager;
                if (cm?.Connectors == null)
                    yield break;

                foreach (Connector c in cm.Connectors)
                    yield return c;
                break;
            }
        }
    }

    private static BoundingBoxXYZ? GetPipeBoundingBox(Pipe pipe)
    {
        var bb = pipe.get_BoundingBox(null);
        if (bb == null)
            return null;

        var min = bb.Min;
        var max = bb.Max;

        bool IsFinite(XYZ p)
        {
            return !(double.IsNaN(p.X) || double.IsNaN(p.Y) || double.IsNaN(p.Z) ||
                     double.IsInfinity(p.X) || double.IsInfinity(p.Y) || double.IsInfinity(p.Z));
        }

        if (!IsFinite(min) || !IsFinite(max))
            return null;

        var result = new BoundingBoxXYZ
        {
            Min = min,
            Max = max
        };

        return result;
    }
    private static bool BoxesIntersect(BoundingBoxXYZ a, BoundingBoxXYZ b, double tol = 1e-6)
    {
        var aMin = a.Min;
        var aMax = a.Max;
        var bMin = b.Min;
        var bMax = b.Max;

        return aMin.X <= bMax.X + tol && aMax.X + tol >= bMin.X &&
               aMin.Y <= bMax.Y + tol && aMax.Y + tol >= bMin.Y &&
               aMin.Z <= bMax.Z + tol && aMax.Z + tol >= bMin.Z;
    }
    private static List<Element> GetIntersectingElementsForEach(IList<Element> connectedElements)
    {
        var resultIds = new HashSet<ElementId>();
        var result = new List<Element>();
        var familyInstances = new FilteredElementCollector(Document)
            .OfClass(typeof(FamilyInstance))
            .WhereElementIsNotElementType()
            .Cast<FamilyInstance>()
            .ToList();

        var famBoxes = new List<(FamilyInstance Fi, BoundingBoxXYZ Box)>();

        foreach (var fi in familyInstances)
        {
            var bb = fi.get_BoundingBox(null);
            if (bb != null)
                famBoxes.Add((fi, bb));
        }

        foreach (var el in connectedElements)
        {
            if (el is not Pipe pipe)
                continue;

            var pipeBox = GetPipeBoundingBox(pipe);

            if (pipeBox == null) continue;

            foreach (var (fi, box) in famBoxes)
            {
                if (!BoxesIntersect(pipeBox, box))
                    continue;
                if (resultIds.Add(fi.Id))
                    result.Add(fi);
            }
        }
        return result;
    }
    public static void Execute(List<Pipe> pipes, string riserName, string parameterName)
    {
        if (pipes == null || pipes.Count == 0)
            return;

        var visited = new HashSet<ElementId>();
        var startPipe = pipes[0];
        var stopId = pipes.Count > 1 ? pipes[1].Id : null;
   

        var connectedElements = GetElementsFromTopConnector(startPipe, visited, stopId);

        foreach (var el in connectedElements)
        {
            var par = el.LookupParameter(parameterName);
            par?.SetParameterValue(riserName);

            if (el is not FamilyInstance fi)
                continue;

            foreach (var subelem in fi.GetAllSubComponents())
            {
                var parSubElem = subelem.LookupParameter(parameterName);
                parSubElem?.SetParameterValue(riserName);
            }
        }

        var intersected = GetIntersectingElementsForEach(connectedElements);
        
        foreach (var el in intersected)
        {
            var par = el.LookupParameter(parameterName);
            par?.SetParameterValue(riserName);

            if (el is not FamilyInstance fi)
                continue;

            foreach (var sub in fi.GetAllSubComponents())
            {
                var parSub = sub.LookupParameter(parameterName);
                parSub?.SetParameterValue(riserName);
            }
        }
    }
}