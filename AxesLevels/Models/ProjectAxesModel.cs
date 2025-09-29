namespace ProjectAxes.Models;

public class ProjectAxesModel : IModel
{
    private readonly Document _doc;

    public ProjectAxesModel(Document doc)
    {
        _doc = doc;
    }

    public async void DoAll(bool beginCheck, bool endCheck)
    {
        await Handler.Handler.AsyncEventHandler.RaiseAsync(async app =>
            {
                using (var t = new Transaction(_doc, "Set HideBubble axes"))
                {
                    t.Start();
                    var grids = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                        .OfCategory(BuiltInCategory.OST_Grids)
                        .WhereElementIsNotElementType()
                        .Cast<Grid>()
                        .ToList();

                    foreach (var grid in grids)
                    {
                        if (!beginCheck)
                            grid.HideBubbleInView(DatumEnds.End0, _doc.ActiveView);
                        else
                            grid.ShowBubbleInView(DatumEnds.End0, _doc.ActiveView);
                        if (!endCheck)
                            grid.HideBubbleInView(DatumEnds.End1, _doc.ActiveView);
                        else
                            grid.ShowBubbleInView(DatumEnds.End1, _doc.ActiveView);
                    }

                    t.Commit();
                }
            }
        );
    }

    public async void DoSelection(bool beginCheck, bool endCheck)
    {
        await Handler.Handler.AsyncEventHandler.RaiseAsync(async app =>
        {
            using (var t = new Transaction(_doc, "Set HideBubble axes (Selection)"))
            {
                t.Start();

                var uidoc = Context.UiApplication.ActiveUIDocument;
                var selectedIds = uidoc.Selection.GetElementIds();

                var grids = selectedIds
                    .Select(id => _doc.GetElement(id))
                    .OfType<Grid>()
                    .ToList();

                foreach (var grid in grids)
                {
                    if (!beginCheck)
                        grid.HideBubbleInView(DatumEnds.End0, _doc.ActiveView);
                    else
                        grid.ShowBubbleInView(DatumEnds.End0, _doc.ActiveView);
                    if (!endCheck)
                        grid.HideBubbleInView(DatumEnds.End1, _doc.ActiveView);
                    else
                        grid.ShowBubbleInView(DatumEnds.End1, _doc.ActiveView);
                }

                t.Commit();
            }
        });
    }
}