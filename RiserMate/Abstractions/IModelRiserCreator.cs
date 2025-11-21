using RiserMate.Entities;

namespace RiserMate.Abstractions;

public interface IModelRiserCreator
{
    public List<string> GetUserParameters();
    public List<HeatingRiser> GetHeatingRisers();
    public void SelectHeatingRiser(HeatingRiser e);
    public void Show3D(HeatingRiser e);
}