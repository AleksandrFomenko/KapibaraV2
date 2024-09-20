using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace FsmModules.ViewModels
{
    public partial class FsmModulesViewModel : ObservableObject
    {
        [RelayCommand]
        private void Execute()
        {
            var uidoc = new UIDocument(Context.ActiveDocument);
            var sel = uidoc.Selection;
            var objectType = ObjectType.Element;
            var status = "Выбери ФСМ модуль";
            var selectedReference = sel.PickObject(objectType, status);
            var selectedElement = Context.ActiveDocument.GetElement(selectedReference);

            var wallType = Context.ActiveDocument.GetElement(new ElementId(398));
            var wallType2 = Context.ActiveDocument.GetElement(new ElementId(401));

            var lvl = new FilteredElementCollector(Context.Document)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .FirstOrDefault() as Level;
            
            var heightParam = selectedElement.LookupParameter("Высота"); 
            double wallHeight = heightParam != null ? heightParam.AsDouble() : 10.0;
            
            var contours = FindContours(selectedElement);
            
            Dictionary<Wall, Curve> walls = new Dictionary<Wall, Curve>();
            
            using (var t = new Transaction(Context.Document, "Create Walls"))
            {
                t.Start();
                foreach (var contour in contours)
                {
                    foreach (var curve in contour)
                    {
                        Wall x = Wall.Create(Context.Document, curve, wallType.Id, lvl.Id, wallHeight, 0, false, false);
                        walls.Add(x, curve);
                    }
                }
                t.Commit();
            }
            
            using (var t = new Transaction(Context.Document, "Create Walls"))
            {
                t.Start();
                foreach (var wal in walls)
                {
                    Wall existingWall = wal.Key;
                    Curve curve = wal.Value;

                    var vector = existingWall.Orientation;
                    double offsetDistance1 = Context.ActiveDocument.GetElement(wal.Key.GetTypeId())
                        .get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble()/2;
                    double offsetDistance2 = wallType2.get_Parameter(BuiltInParameter.WALL_ATTR_WIDTH_PARAM).AsDouble()/2;
                    double offsetDistance = offsetDistance1 + offsetDistance2;

                    Transform translation = Transform.CreateTranslation(vector * offsetDistance);

                    Curve newCurve = curve.CreateTransformed(translation);

                    Wall newWall = Wall.Create(Context.Document, newCurve, wallType2.Id, lvl.Id, wallHeight, 0, false, false);
                }
                t.Commit();
            }
        }

        private static IEnumerable<CurveLoop> FindContours(Element element)
        {
            return GetSolids(element)
                .SelectMany(solid => GetContours(solid, element));
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
    }
}
