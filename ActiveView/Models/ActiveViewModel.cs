using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using KapibaraCore.Parameters;

namespace ActiveView.Models;

public class ActiveViewModel : IModelActiveView
{
    private Document _doc;
    public ActiveViewModel(Document doc)
    {
        _doc = doc;
    }

    public List<string> GetParameters()
    {
        return _doc.GetProjectParameters();
    }

    public void Execute(string parameterName, string value, bool skipNotEmpty)
    {
        var elems = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
            .WhereElementIsNotElementType()
            .ToElements();
        using (var t = new Transaction(_doc,"ActiveView"))
        {
            t.Start();
            foreach (var elem in elems) 
            { 
                var parameter = elem.GetParameterByName(parameterName);
                if (skipNotEmpty)
                {
                   if(CheckParameterValue(parameter)) continue;
                }
                parameter.SetParameterValue(value);
            }
            t.Commit();
        }
    }

    private static bool CheckParameterValue(Parameter parameter)
    {
        if (parameter is null) return true;
        if (!parameter.HasValue) return false;
    
        return !string.IsNullOrEmpty(parameter.AsValueString());
    }
}