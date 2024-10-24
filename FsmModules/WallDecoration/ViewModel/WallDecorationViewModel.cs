using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using FsmModules.Model;
using FsmModules.Modules;


namespace FsmModules.WallDecoration.ViewModel;


public class WallDecorationViewModel : IWorker
{
    private Document _doc;
    private ObjectType _objectType = ObjectType.Element;
    private string _status = "Выберите Прехаб модуль";
    internal WallDecorationViewModel(Document doc)
    {
        _doc = doc;
        
    }
    
    /*
    public void Start()
    {
        
        var uidoc = new UIDocument(_doc);
        var sel = uidoc.Selection;
        var selectedReference = sel.PickObjects(_objectType, _status);

        var wallType = Context.ActiveDocument.GetElement(new ElementId(398)).Cast<WallType>();
        var wallType2 = Context.ActiveDocument.GetElement(new ElementId(401)).Cast<WallType>();
        var wallType3 = Context.ActiveDocument.GetElement(new ElementId(400)).Cast<WallType>();
        var wallType4 = Context.ActiveDocument.GetElement(new ElementId(9426)).Cast<WallType>();



        var lvl = new FilteredElementCollector(_doc)
            .OfClass(typeof(Level))
            .WhereElementIsNotElementType()
            .FirstOrDefault() as Level;
        ModulesBase q = new Modules.FacadeModule.FacadeModule(_doc);

        using var t = new Transaction(_doc, "Create Walls");
        foreach (var selectedRef in selectedReference)
        {
            t.Start();
            var selectedElement = _doc.GetElement(selectedRef);
            var dic = q.CreateExternalWalls(selectedElement, wallType, lvl, 100);
    
            t.Commit();
            dic = q.CreateInternalWalls(dic, wallType2, lvl, 100);
            dic = q.CreateInternalWalls(dic, wallType3, lvl, 100);
            dic = q.CreateInternalWalls(dic, wallType4, lvl, 100);
        }

    }
    */
    public void Start()
    {
        throw new NotImplementedException();
    }
}