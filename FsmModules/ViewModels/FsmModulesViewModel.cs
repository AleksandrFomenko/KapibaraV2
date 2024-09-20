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

            var wallType = Context.ActiveDocument.GetElement(new ElementId(6291));

            var lvl = new FilteredElementCollector(Context.Document)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .FirstOrDefault() as Level;
            
            var heightParam = selectedElement.LookupParameter("Высота"); 
            double wallHeight = heightParam != null ? heightParam.AsDouble() : 10.0;
            
            var contours = FindContours(selectedElement);

            using (var t = new Transaction(Context.Document, "Create Walls"))
            {
                t.Start();
                foreach (var contour in contours)
                {
                    foreach (var curve in contour)
                    {
                        Wall.Create(Context.Document, curve, wallType.Id, lvl.Id, wallHeight, 0, false, false);
                    }
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
