﻿using System.Diagnostics;
using System.Text;
using Autodesk.Revit.UI;
using KapibaraCore.Parameters;
using KapibaraCore.Solids;
using LevelByFloor.ViewModels;
using System.Globalization;

namespace LevelByFloor.Models;

internal class LevelByFloorModel
{
    private Document _doc;
    private Options _options;

    internal LevelByFloorModel(Document doc)
    {
        _doc = doc;
    }

    internal void SetOpt(Options opt)
    {
        _options = opt;
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
        if (height == 0) TaskDialog.Show("err", "Два уровня имеют одинаковую высоту");
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
        
        var elements = new FilteredElementCollector(_doc)
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
            return 0;
        }
        try
        {
            var intersection = BooleanOperationsUtils.ExecuteBooleanOperation(
                solid1, solid2, BooleanOperationsType.Intersect);
            if (intersection != null && intersection.Volume > 0) return intersection.Volume;

        }
        catch (Exception)
        {
            return 0;
        }

        return 0;
    }
    private Dictionary<ElementId, int> CreateLevelSolids(BoundingBoxXYZ bb, string indent)
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
        var x = levelsBelowZero.Count;
        double.TryParse(indent, NumberStyles.Any, CultureInfo.InvariantCulture, out double indentValue);
        indentValue = indentValue / 304.8;
        for (var i = 0; i < levels.Count - 1; i++)
        {
            if (q < x)
            {
                shape = CreateLevelSolid(bb,
                    levels[i].ProjectElevation + indentValue, 
                    levels[i + 1].ProjectElevation + indentValue);
                dict[shape.Id] = -(x - q);
                q++;
                continue;
            }
            shape = CreateLevelSolid(bb,
                levels[i].ProjectElevation + indentValue,
                levels[i + 1].ProjectElevation + indentValue);
            dict[shape.Id] = i - x + 1;
        }
        var lastLevelElevation = levels[levels.Count - 1].ProjectElevation;
        shape = CreateLevelSolid(bb, lastLevelElevation + indentValue, lastLevelElevation + 500 + indentValue);
        dict[shape.Id] = levels.Count - x;
            
        var firstLevelElevation = levels[0].ProjectElevation;
        shape = CreateLevelSolid(bb, firstLevelElevation - 1000 + indentValue, firstLevelElevation + indentValue);
        dict[shape.Id] = -(x + 1);
        return dict;
    }
    internal void Execute(string parameter, string suffix, string prefix, string indent)
    {
        var elems = _options.Fec.WhereElementIsNotElementType().ToElements();
        if (_doc.ActiveView is not View3D)
        {
            TaskDialog.Show("", "Активный вид не является 3D видом");
            LevelByFloorViewModel.Close();
        }
        try
        {
            Dictionary<ElementId, int> dictionary;
            using (var t1 = new Transaction(_doc, "Create solids"))
            {
                t1.Start();
                dictionary = CreateLevelSolids(GetBoundingBoxForAllElements(), indent);
                t1.Commit();
            }
            using (var t2 = new Transaction(_doc, "Set level"))
            {
                t2.Start();
                Element resultElem = null;
                if(dictionary == null) return;
                foreach (var elem in elems)
                {
                    double maxVolume = 0;
                    foreach (var dict in dictionary)
                    {
                        var checkElem = _doc.GetElement(dict.Key);
                        if(checkElem == null) continue;
                        
                        var volume = CheckIntersection(elem, checkElem);
                        if (!(volume > maxVolume)) continue;
                        maxVolume = volume;
                        resultElem = checkElem;
                    }

                    if (resultElem == null) continue;
                    var par = elem.GetParameterByName(parameter);
                    if (par==null) continue;
                    var builder = new StringBuilder();
                    builder.Append(prefix);
                    builder.Append(dictionary[resultElem.Id].ToString());
                    builder.Append(suffix);
                    par.SetParameterValue(builder.ToString());
                }
                t2.Commit();
            }
            using (var t3 = new Transaction(_doc, "Delete solids"))
            {
                t3.Start();
                foreach (var dict in dictionary)
                {
                    _doc.Delete(dict.Key);
                }
                t3.Commit();
            }
        }
        catch (Exception e)
        {
            TaskDialog.Show("1", e.ToString());
            Debug.WriteLine(e.ToString());
        }
    }
}