using KapibaraCore.Parameters;
using RiserMate.Abstractions;
using RiserMate.Core;
using RiserMate.Entities;


namespace RiserMate.Models.Revit;

public class ModelRiserMateCreator : IModelRiserCreator
{
    private readonly Document? _document = Context.ActiveDocument;
    public List<string> GetUserParameters() => _document.GetProjectParameters(BuiltInCategory.OST_PipeCurves);
    
    public List<HeatingRiser> GetHeatingRisers(string parameter)
    {
        var pipes = new FilteredElementCollector(_document)
            .OfCategory(BuiltInCategory.OST_PipeCurves)
            .WhereElementIsNotElementType()
            .ToElements();
        
        var hashSet = new HashSet<string>();

        foreach (var pipe in pipes)
        {
            var rise = GetParameterValue(pipe, parameter);
            if (rise == string.Empty) continue;
            hashSet.Add(GetParameterValue(pipe, parameter));
        }
        
        return hashSet.Select(p => new HeatingRiser(p)).ToList();
    }

    private static string GetParameterValue(Element elem, string parameterName) => 
        elem.LookupParameter(parameterName)?.AsString() ?? string.Empty;
    
    public void SelectHeatingRiser(HeatingRiser e, string parameter)
    {
        var elements = new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .Where(pipe => pipe.LookupParameter(parameter)?.AsString() == e.Name)
            .ToList();

        var sel = Context.ActiveUiDocument?.Selection;

        sel?.SetElementIds(elements.Select(x => x.Id).ToList());
    }

    public void Show3D(HeatingRiser e)
    {
        Console.WriteLine("Show3D");
    }

    public async void Execute(List<HeatingRiser> heatingRisers, string parameterName)
    {
        await Handlers.Handlers.AsyncEventHandler.RaiseAsync(async app =>
        {
            
            using var t = new Transaction(_document, "RiserMate");
            t.Start();
            foreach (var heatingRiser in heatingRisers)
            {
                var pip = RiserMateCore.GetBottomPipesByRiser(heatingRiser.Name, parameterName);

                if (pip == null) continue;
                RiserMateCore.Execute(pip, heatingRiser.Name, parameterName);
                
            }

            t.Commit();
        });
    }
}