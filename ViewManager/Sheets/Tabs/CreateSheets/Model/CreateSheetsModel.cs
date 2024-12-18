using System.Diagnostics;
using ViewManager.ViewModels;

namespace ViewManager.Sheets.Tabs.CreateSheets.Model;

internal class CreateSheetsModel
{
    internal CreateSheetsData _data;
    private Document _doc;
    internal CreateSheetsModel(Document doc)
    {
        _doc = doc;
        _data = new CreateSheetsData(doc);
    }

    internal void Make(ElementId titleBlockTypeId, int count, int startValue)
    {
        Autodesk.Revit.ApplicationServices.Application app = _doc.Application;
        FailureProcessorNumerator failureProcessor = new FailureProcessorNumerator();
        app.FailuresProcessing +=  failureProcessor.ApplicationOnFailuresProcessing; 
        
        for (int i = 0; i < count; i++)
        {
            var view = ViewSheet.Create(_doc, titleBlockTypeId);
            SetSystemNumeration(view, (startValue).ToString());
        }
        app.FailuresProcessing -=  failureProcessor.ApplicationOnFailuresProcessing; 
    }

    private void SetSystemNumeration(Autodesk.Revit.DB.View view, string startValue)
    {
        var parameter = view.get_Parameter(BuiltInParameter.SHEET_NUMBER);
        parameter?.Set(startValue);
    }
    internal void MakeSheets(Action act, string name)
    {
        using (Transaction t = new Transaction(_doc, name))
        {
            t.Start();
            act();
            t.Commit();
        }
        ViewManagerViewModel.CloseWindow?.Invoke();
    }
}

