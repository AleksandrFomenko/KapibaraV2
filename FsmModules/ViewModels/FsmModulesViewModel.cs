using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using FsmModules.Modules.FacadeModule;


namespace FsmModules.ViewModels
{
    public partial class FsmModulesViewModel : ObservableObject
    {
        private Document _doc = Context.Document;
        private const ObjectType objectType = ObjectType.Element;
        private const string status = "Выбери ФСМ модуль";
        [RelayCommand]
        private void Execute(Window window)
        {
            window?.Close();
            var uidoc = new UIDocument(_doc);
            var sel = uidoc.Selection;
            var selectedReference = sel.PickObjects(objectType, status);

            var wallType = Context.ActiveDocument.GetElement(new ElementId(398)).Cast<WallType>();
            var wallType2 = Context.ActiveDocument.GetElement(new ElementId(401)).Cast<WallType>();
            var wallType3 = Context.ActiveDocument.GetElement(new ElementId(400)).Cast<WallType>();

            
            var lvl = new FilteredElementCollector(Context.Document)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .FirstOrDefault() as Level;
            var q = new FacadeModule(_doc);

            using var t = new Transaction(Context.Document, "Create Walls");
            foreach (var selectedRef in selectedReference)
            {
                t.Start();
                var selectedElement = _doc.GetElement(selectedRef);
                var dic = q.CreateExternalWalls(selectedElement, wallType, lvl, 500);
                t.Commit();
                var dic2 = q.CreateInternalWalls(dic, wallType2, lvl, 500);
                var dic3 = q.CreateInternalWalls(dic2, wallType3, lvl, 500);
            }
        }
    }
}



