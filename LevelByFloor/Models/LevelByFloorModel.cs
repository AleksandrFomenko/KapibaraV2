using System.Diagnostics;
using Autodesk.Revit.UI;
using KapibaraCore.Parameters;
using KapibaraCore.Solids;
using LevelByFloor.ViewModels;

namespace LevelByFloor.Models;

internal class LevelByFloorModel
{
    private Document _doc;

    internal LevelByFloorModel(Document doc)
    {
        _doc = doc;
    }

    internal List<string> LoadParameters()
    {
        return _doc.GetProjectParameters(SpecTypeId.String.Text).ToList();
    }
    private Solid CreateSolidFromBoundingBox(BoundingBoxXYZ bb)
    {
        if (bb == null) return null;
        var min = bb.Min;
        var max = bb.Max;
        
        var p0 = new XYZ(min.X, min.Y, min.Z); // нижний левый угол
        var p1 = new XYZ(max.X, min.Y, min.Z); // Нижний правй угол
        var p2 = new XYZ(max.X, max.Y, min.Z); // Верхний правый угол
        var p3 = new XYZ(min.X, max.Y, min.Z); // Верхний левый угол
        
        var bottomEdges = new List<Curve>
        {
            Line.CreateBound(p0, p1),
            Line.CreateBound(p1, p2),
            Line.CreateBound(p2, p3),
            Line.CreateBound(p3, p0)
        };
        var bottomLoop = CurveLoop.Create(bottomEdges);
        
        var height = max.Z - min.Z;
        
        var solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { bottomLoop }, XYZ.BasisZ, height);

        return solid;
    }
    private DirectShape CreateDirectShape(GeometryObject solid, string categoryName)
    {
        var categoryId = _doc.Settings.Categories
            .Cast<Category>()
            .Where(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Id)
            .FirstOrDefault();
        var geometryElement = new List<GeometryObject>() { solid };
        var shape = DirectShape.CreateElement(_doc, categoryId);
        shape.SetShape(geometryElement);
        return shape;
    }
    private BoundingBoxXYZ GetBoundingBoxForAllElements()
    {
        BoundingBoxXYZ combinedBoundingBox = new BoundingBoxXYZ();
        bool isFirstBoundingBox = true;
        
        var elements = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
            .WhereElementIsNotElementType()
            .ToElements();

        foreach (var element in elements)
        {
            var elementBoundingBox = element.get_BoundingBox(null);
            if (elementBoundingBox == null) continue;
            
            if (isFirstBoundingBox)
            {
                combinedBoundingBox.Min = elementBoundingBox.Min;
                combinedBoundingBox.Max = elementBoundingBox.Max;
                isFirstBoundingBox = false;
            }
            else
            {
                combinedBoundingBox.Min = new XYZ(
                    Math.Min(combinedBoundingBox.Min.X, elementBoundingBox.Min.X),
                    Math.Min(combinedBoundingBox.Min.Y, elementBoundingBox.Min.Y),
                    Math.Min(combinedBoundingBox.Min.Z, elementBoundingBox.Min.Z)
                );

                combinedBoundingBox.Max = new XYZ(
                    Math.Max(combinedBoundingBox.Max.X, elementBoundingBox.Max.X),
                    Math.Max(combinedBoundingBox.Max.Y, elementBoundingBox.Max.Y),
                    Math.Max(combinedBoundingBox.Max.Z, elementBoundingBox.Max.Z)
                );
            }
        }
        return isFirstBoundingBox ? null : combinedBoundingBox;
    }

    private DirectShape CreateLevelSolid(BoundingBoxXYZ bb, double z2, double z1)
    {
        var newBb = new BoundingBoxXYZ();
        var maxXyz = new XYZ(bb.Max.X, bb.Max.Y, z1);
        var minXyz = new XYZ(bb.Min.X, bb.Min.Y, z2);
        newBb.Max = maxXyz;
        newBb.Min = minXyz;
        var solid = CreateSolidFromBoundingBox(newBb);
        var shape = CreateDirectShape(solid, "Оборудование");
        return shape;
    }
    private double CheckIntersection(Element element1, Element element2)
    {
        var solid1 = element1.GetSolid();
        var solid2 = element2.GetSolid();

        if (solid1 == null || solid2 == null)
        {
            //Debug.WriteLine("Один из элементов не имеет валидной геометрии.");
            return 0;
        }
        try
        {
            var intersection = BooleanOperationsUtils.ExecuteBooleanOperation(
                solid1, solid2, BooleanOperationsType.Intersect);
            if (intersection != null && intersection.Volume > 0) return intersection.Volume;

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка при пересечении: {ex.Message}");
            return 0;
        }

        return 0;
    }
    private Dictionary<ElementId, int> CreateLevelSolids(BoundingBoxXYZ bb)
    {
        var dict = new Dictionary<ElementId, int>();
        var levels = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .Cast<Level>()
            .OrderBy(level => level.ProjectElevation)
            .ToList();
        var levelsBelowZero = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .Cast<Level>()
            .Where(level => Math.Round(level.ProjectElevation) < 0)
            .ToList();
        if (!levels.Any())
        {
            TaskDialog.Show("", "На виде отсутствуют уровни");
            LevelByFloorViewModel.Close();
            return new Dictionary<ElementId, int>();
        }

        DirectShape shape = null;
        var q = 0;
        for (int i = 0; i < levels.Count - 1; i++)
        {
            
            var x = levelsBelowZero.Count;
            if (q < x)
            {
                shape = CreateLevelSolid(bb, levels[i].ProjectElevation, levels[i + 1].ProjectElevation);
                dict[shape.Id] = -(x - q);
                q++;
                continue;
            }
            shape = CreateLevelSolid(bb, levels[i].ProjectElevation, levels[i + 1].ProjectElevation);
            dict[shape.Id] = i - x + 1;
        }
        var lastLevelElevation = levels[levels.Count - 1].ProjectElevation;
        CreateLevelSolid(bb, lastLevelElevation, lastLevelElevation + 500);
            
        var firstLevelElevation = levels[0].ProjectElevation;
        CreateLevelSolid(bb, firstLevelElevation - 1000, firstLevelElevation);
        return dict;
    }
    internal void Execute()
    {
        var elems = new FilteredElementCollector(_doc).WhereElementIsNotElementType().ToElements();
        if (_doc.ActiveView is not View3D view3D)
        {
            TaskDialog.Show("", "Активный вид не является 3D видом");
            LevelByFloorViewModel.Close();
        }
        try
        {
            using (var t = new Transaction(_doc, "Set level"))
            {
                t.Start();
                var dictionary = CreateLevelSolids(GetBoundingBoxForAllElements());
                foreach (var dict in dictionary)
                {
                    Debug.Write($"Ключ {dict.Key.ToString()}, значение {dict.Value.ToString()}");
                }

                Element resultElem = null;
                double maxVolume = 0;
                foreach (var elem in elems)
                {
                    foreach (var dict in dictionary)
                    {
                        var checkElem = _doc.GetElement(dict.Key);
                        
                        var volume = CheckIntersection(elem, checkElem);
                        if (volume > maxVolume)
                        {
                            maxVolume = volume;
                            resultElem = checkElem;
                        }
                    }
                    if (resultElem != null)
                    {
                        
                    }
                    var par = elem.LookupParameter("тест");
                    if (par != null)
                    {
                        par.Set("выав");
                    }
                }
                t.Commit();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
        }
    }
}