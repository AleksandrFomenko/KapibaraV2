namespace ProjectAxes.Models;

public class ProjectLevelsModel : IModel
{
    private readonly Document _doc;

    public ProjectLevelsModel(Document doc)
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
                    var levels = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                        .OfCategory(BuiltInCategory.OST_Levels)
                        .WhereElementIsNotElementType()
                        .Cast<Level>()
                        .ToList();

                    foreach (var level in levels)
                    {
                        if (!beginCheck)
                            level.HideBubbleInView(DatumEnds.End0, _doc.ActiveView);
                        else
                            level.ShowBubbleInView(DatumEnds.End0, _doc.ActiveView);
                        if (!endCheck)
                            level.HideBubbleInView(DatumEnds.End1, _doc.ActiveView);
                        else
                            level.ShowBubbleInView(DatumEnds.End1, _doc.ActiveView);
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

                var levels = selectedIds
                    .Select(id => _doc.GetElement(id))
                    .OfType<Level>()
                    .ToList();

                foreach (var level in levels)
                {
                    if (!beginCheck)
                        level.HideBubbleInView(DatumEnds.End0, _doc.ActiveView);
                    else
                        level.ShowBubbleInView(DatumEnds.End0, _doc.ActiveView);

                    if (!endCheck)
                        level.HideBubbleInView(DatumEnds.End1, _doc.ActiveView);
                    else
                        level.ShowBubbleInView(DatumEnds.End1, _doc.ActiveView);
                }

                t.Commit();
            }
        });
    }
}