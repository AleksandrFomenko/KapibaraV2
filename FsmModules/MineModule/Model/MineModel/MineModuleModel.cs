using FsmModules.Model.solidHandler;
using Autodesk.Revit.DB.Structure;

namespace FsmModules.MineModule.Model.MineModel;

public class MineModuleModel 
{
    private Document _doc;
    private SolidHandler _solidHandler;
    private StructuralType _structuralType = StructuralType.NonStructural;

    private const string _parameterHeight = "ADSK_Размер_Высота";
    private const string _parameterLength = "ADSK_Размер_Длина";
    private const string _parameterWidth = "ADSK_Размер_Ширина";

    
    internal MineModuleModel(Document doc)
    { 
        _solidHandler = new SolidHandler();
        _doc = doc;
    }

    private Level searchLevel(string levelName)
    {
        var lvl = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_Levels)
            .WhereElementIsNotElementType()
            .Cast<Level>()
            .FirstOrDefault(l => l.Name == levelName);
        return lvl;
    }
    
    private FamilySymbol searchFamily(string familyName)
    {
        var elem = new FilteredElementCollector(_doc)
            .WhereElementIsElementType()
            .FirstOrDefault(l => l.Name == familyName);
        return elem as FamilySymbol;
    }
    private IEnumerable<Level> GetLevelsBetween(double zStart, double zEnd)
    {
        double minZ = Math.Min(zStart, zEnd);
        double maxZ = Math.Max(zStart, zEnd);
        
        var allLevels = new FilteredElementCollector(_doc)
            .OfClass(typeof(Level))
            .Cast<Level>();
        
        var levelsBetween = allLevels
            .Where(l => 
            {
                double elev = l.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
                return elev >= minZ && elev <= maxZ;
            })
            .OrderBy(l => l.Elevation)
            .ToList();

        return levelsBetween;
    }

    private void RotateElement(FamilyInstance instanceBase, FamilyInstance instanceNew)
    {
        var locationPoint = instanceBase.Location as LocationPoint;
        XYZ xyz1 = new XYZ(locationPoint.Point.X, locationPoint.Point.Y, locationPoint.Point.Z+1);
        Line line = Line.CreateBound(locationPoint.Point, xyz1);
                  
        var facingOrientationNewFamily = instanceNew.FacingOrientation;
        var facingOrientationFamily = instanceBase.FacingOrientation;
        
        var angle = facingOrientationNewFamily.AngleTo(facingOrientationFamily);
        if (facingOrientationFamily.X < 0)
        {
            angle = 2 * Math.PI - angle;
        }

        ElementTransformUtils.RotateElements(_doc,  new[] { instanceNew.Id }, line, -angle);
        
    }

    public void Execute(Element elem, string nameFamilyStart, string nameFamilyEnd, string nameLevelStart, 
        string nameLevelEnd, string nameFamilyTransfer)
    {
        
        var solid = _solidHandler.GetSolids(elem).OrderByDescending(s => s.Volume).FirstOrDefault();
        var centroid = solid.ComputeCentroid();
        var levelStart = searchLevel(nameLevelStart);
        var levelEnd = searchLevel(nameLevelEnd);
        var zStart = levelStart.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
        var zEnd = levelEnd.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();

        var step = searchFamily(nameFamilyStart).LookupParameter(_parameterHeight).AsDouble();

        var levelsBetween = GetLevelsBetween(zStart, zEnd).ToList();
        var resultZ = levelsBetween
            .Aggregate((max, current) => current.Elevation > max.Elevation ? current : max);
        
        var familyInstance = elem as FamilyInstance;

        
        for (int i = 0; i < levelsBetween.Count() - 1; i++)
        {
            var start1 =  levelsBetween[i].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            var end1 =  levelsBetween[i+1].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            var end = end1 - start1;
            var res = Math.Floor(end / step);
            for (double z = 0; z < res; z++)
            {
                var xyz = new XYZ(centroid.X, centroid.Y, z*step);
                var newInstance = _doc.Create.NewFamilyInstance(xyz, searchFamily(nameFamilyStart), levelsBetween[i],
                    _structuralType);
                RotateElement(familyInstance, newInstance);

            }
        }
        
        var higherLevels = new FilteredElementCollector(_doc)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .Where(l => l.Elevation >= resultZ.Elevation)
            .OrderBy(l => l.Elevation)
            .ToList();
        
        step = searchFamily(nameFamilyEnd).LookupParameter(_parameterHeight).AsDouble();
        var stepTransfer = searchFamily(nameFamilyTransfer).LookupParameter(_parameterHeight).AsDouble();

        for (int i = 0; i < higherLevels.Count() - 1; i++)
        {
            var start1 = higherLevels[i].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            var end1 =  higherLevels[i+1].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            
            var end = end1 - start1;
            var res = 0.0;
            if (i == 0)
            {
                res = Math.Floor((end- stepTransfer)/ step);
            }
            else
            {
                res = Math.Floor(end/ step);
            }
            
            var height = 0.0;  
            for (double z = 0; z < res; z++)
            {
                var xyz = new XYZ(centroid.X, centroid.Y, height);
                
                if (z == 0 && i == 0)
                {
                    var newTransfer = _doc.Create.NewFamilyInstance(xyz, searchFamily(nameFamilyTransfer), higherLevels[i], _structuralType);
                    RotateElement(familyInstance, newTransfer);
                    height += stepTransfer;
                    continue;

                }
                var newInstance = _doc.Create.NewFamilyInstance(xyz, searchFamily(nameFamilyEnd), higherLevels[i], _structuralType);
                RotateElement(familyInstance, newInstance);
                height += step;
            }
        }
    }
}