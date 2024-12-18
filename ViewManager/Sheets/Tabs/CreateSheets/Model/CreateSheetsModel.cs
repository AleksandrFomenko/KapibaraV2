using System.Diagnostics;
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

    internal void Make(ElementId titleBlockTypeId, int count, int startValue, bool isUserPar, string userParameter)
    {
        for (int i = 0; i < count; i++)
        {
            var uniqueNumber = GetUnique("‪"+"‪"+"‪"+"‪"+"‪"+(startValue+i).ToString());
            var view = ViewSheet.Create(_doc, titleBlockTypeId);
            SetSystemNumeration(view, uniqueNumber);
            if (isUserPar) SetUserNumeration(view, userParameter, (startValue+i).ToString());
        }
    }

    private void SetSystemNumeration(Autodesk.Revit.DB.View view, string Value)
    {
        var parameter = view.get_Parameter(BuiltInParameter.SHEET_NUMBER);
  
        parameter?.Set(Value);
    }
    private void SetUserNumeration(Autodesk.Revit.DB.View view, string parameterName, string Value)
    {
        var parameter = Parameters.GetParameterByName(_doc, view, parameterName);
        parameter.Set("аqwewqe");
        Parameters.SetParameterValue(parameter, Value);
    }
    
    public static Parameter GetParameterByName(Document doc, object elem, string parameterName)
    { 
        if (elem is Element element)
        {
            Debug.WriteLine("это элемент");
            if (elem is FamilyInstance instance)
            {
                var par = element.LookupParameter(parameterName);
                Debug.WriteLine("1");
                if (par != null) return par;
                var type = doc.GetElement(instance.GetTypeId());
                par = type?.LookupParameter(parameterName);
                if (par != null) return par;
            }
            return element.LookupParameter(parameterName);
        }
        else
        {
            Debug.WriteLine("это не элемент");
        }

        return null;
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

