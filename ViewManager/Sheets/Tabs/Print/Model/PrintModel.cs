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

        foreach (var sheetId in sheets)
        {
            var sh = _doc.GetElement(sheetId);
            if (sh is ViewSheet viewSheet)
            {
                var sheet1 = new List<ElementId> { sheetId };
                settings.Combine = false;
                _doc.Export(path, sheet1, settings);
            }
        }
    }
}