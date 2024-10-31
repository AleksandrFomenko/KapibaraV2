namespace ViewManager.Legends.Model;

internal class LegendsModel
{
    private Document _doc;
    private Autodesk.Revit.DB.View _legend;
    internal LegendsModel(Document doc, Autodesk.Revit.DB.View Legend)
    {
        _legend = Legend;
        _doc = doc;
    }

    internal void Execute(List<Autodesk.Revit.DB.ViewSheet> views, string position, double xChange, double yChange)
    {
        foreach (var view in views)
        {
            var outline = view.Outline;
            double x = 0;
            double y = 0;
            switch (position)
            {
                case "Правый нижний":
                    x = outline.Max.U - xChange;
                    y = outline.Min.V + yChange;
                    break;
                case "Правый верхний":
                    x = outline.Max.U - xChange;
                    y = outline.Max.V - yChange;
                    break;
                case "Левый нижний":
                    x = outline.Min.U + xChange;
                    y = outline.Min.V + yChange;
                    break;
                case "Левый верхний":
                    x = outline.Min.U + xChange;
                    y = outline.Max.V - yChange;
                    break;
            }
            
            var xyz =  new XYZ(x, y, 0);
            try
            {
                Viewport.Create(_doc, view.Id, _legend.Id, xyz);
            }
            catch
            {
                
            }
        }
    }
}