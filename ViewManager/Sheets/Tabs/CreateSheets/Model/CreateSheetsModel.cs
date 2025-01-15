using KapibaraCore.Parameters;
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
    
    internal void MakeSheets(Action act, string name)
    {
        using (Transaction t = new Transaction(_doc, name))
        {
            t.Start();
            
            var failHandOpts = t.GetFailureHandlingOptions();
            var u = new NumeratorWarnings();
            failHandOpts.SetClearAfterRollback(true);
            failHandOpts.SetFailuresPreprocessor(u);
            t.SetFailureHandlingOptions(failHandOpts);
            act();
            
            t.Commit();

        }
        ViewManagerViewModel.CloseWindow?.Invoke();
    }

    internal void Make(ElementId titleBlockTypeId, int count, int startValue, bool isSystemPar, bool isUserPar, string userParameter)
    {
        for (int i = 0; i < count; i++)
        {
            var uniqueNumber = GetUnique("‪"+"‪"+"‪"+"‪"+"‪"+(startValue+i).ToString());
            var view = ViewSheet.Create(_doc, titleBlockTypeId);
            
            if (isSystemPar) SetSystemNumeration(view, uniqueNumber);
            if (isUserPar) SetUserNumeration(view, userParameter, (startValue+i).ToString());
        }
    }

    private void SetSystemNumeration(Autodesk.Revit.DB.View view, string value)
    {
        var parameter = view.get_Parameter(BuiltInParameter.SHEET_NUMBER);
  
        parameter?.Set(value);
    }
    private void SetUserNumeration(Element elem, string parameterName, string value)
    {
        var parameter = Parameters.GetParameterByName(elem, parameterName);
        if (parameter == null)
        {
            if (elem is Autodesk.Revit.DB.View view)
            {
                var sheets = new FilteredElementCollector(_doc, view.Id)
                    .WhereElementIsNotElementType().ToElements().FirstOrDefault();
                parameter = Parameters.GetParameterByName(sheets, parameterName);
                
            }
        }
        
        Parameters.SetParameterValue(parameter, value);
    }
    
    private string GetUnique(string startValue)
    {
        var views = new FilteredElementCollector(_doc)
            .OfClass(typeof(ViewSheet))
            .WhereElementIsNotElementType()
            .ToElements();

        bool exists = views.Any(v =>
        {
            var parameter = v.get_Parameter(BuiltInParameter.SHEET_NUMBER);
            return parameter != null && parameter.AsValueString() == startValue;
        });

        if (exists)
        {
            return GetUnique("‪"+startValue );
        }
        return startValue;
    }
}

