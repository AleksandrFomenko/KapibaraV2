using RiserMate.Entities;

namespace RiserMate.Abstractions;

public interface IModelRiserCreator
{
    
    public List<string> GetUserParameters();
    public List<HeatingRiser> GetHeatingRisers(string parameter);
    public void SelectHeatingRiser(HeatingRiser e, string parameter);
    public void Show3D(HeatingRiser e);
    public void Execute(List<HeatingRiser> heatingRisers, string parameter);
}