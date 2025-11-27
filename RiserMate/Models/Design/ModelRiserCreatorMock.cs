using RiserMate.Abstractions;
using RiserMate.Entities;

namespace RiserMate.Models.Design;

public class ModelRiserCreatorMock: IModelRiserCreator
{
    public List<string> GetUserParameters()
    {
        return ["Параметр 1", "Параметр 2", "Параметр 3"];
    }

    public List<HeatingRiser> GetHeatingRisers(string name)
    {
        var result = new List<HeatingRiser>();

        for (var i = 0; i < 3; i++)
        {
            result.Add(new HeatingRiser($"Ст {i}"));
        }
        return result;
    }

    public void SelectHeatingRiser(HeatingRiser e, string parameter) => Console.WriteLine($"Выделить трубу у {e.Name}");
    public void Show3D(HeatingRiser e) => Console.WriteLine($"Показал {e.Name}");
    public void Execute(List<HeatingRiser> heatingRisers, string parameter) => Console.WriteLine("Execute");
    
}