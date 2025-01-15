using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using KapibaraCore.Parameters;

namespace ActiveView.Models;

internal class ActiveViewModel
{
    private Document _doc;
    internal ActiveViewModel(Document doc)
    {
        _doc = doc;
    }

    internal List<string> GetParameters()
    {
        return _doc.GetProjectParameters();
    }
    
    internal void Execute(string parameterName, string value)
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
                parameter.SetParameterValue(value);
            }
            t.Commit();
        }
    }
}