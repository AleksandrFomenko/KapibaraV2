using ActiveView.Handler;
using ActiveView.ViewModels;
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

    public async void Execute(string parameterName, string value, bool skipNotEmpty, Option option)
    {
        await Handlers.AsyncEventHandler.RaiseAsync(async app =>
        {
            using (var tr = new Transaction(_doc, "ActiveView")) 
            { 
                tr.Start();
                
                ICollection<Element> elems;

                if (option.IsAll)
                {
                    elems = new FilteredElementCollector(_doc, _doc.ActiveView.Id)
                        .WhereElementIsNotElementType()
                        .ToElements();
                }
                else
                {
                    elems = Context.ActiveUiDocument!.Selection
                        .GetElementIds()
                        .Select(id => _doc.GetElement(id))
                        .Where(e => e is not null)
                        .ToList();
                }

                foreach (var elem in elems)
                {
                    var parameter = elem.GetParameterByName(parameterName);
                    if (parameter is null) continue;

                    if (skipNotEmpty && CheckParameterValue(parameter))
                        continue;

                    parameter.SetParameterValue(value);
                }
                
                tr.Commit();
            }
        });
    }


    private static bool CheckParameterValue(Parameter parameter)
    {
        if (parameter is null) return true;
        if (!parameter.HasValue) return false;
    
        return !string.IsNullOrEmpty(parameter.AsValueString());
    }
}
