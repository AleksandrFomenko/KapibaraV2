using System.Diagnostics;
using Autodesk.Revit.UI.Events;

namespace ViewManager.Sheets.Tabs.Print.Model;

internal class PrintModel
{
    private Document _doc;
    internal PrintModel(Document doc)
    {
        _doc = doc;
    }

    internal void Execute(string path,List<ElementId> sheets,bool combine, string fileName)
    {
        var settings = new PDFExportOptions();
        if (combine)
        {
            settings.Combine = true;
            settings.FileName = fileName;
            _doc.Export(path, sheets, settings);
            return;
        }

        foreach (var sheet in sheets)
        {
            var sh = _doc.GetElement(sheet);
            if (sh is ViewSheet viewSheet)
            {
                settings.FileName = viewSheet.SheetNumber + " " + viewSheet.Name;
                _doc.Export(path, sheets, settings);
            }
        }
    }
}