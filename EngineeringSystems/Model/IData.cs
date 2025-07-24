using EngineeringSystems.ViewModels;

namespace EngineeringSystems.Model;

public interface IData
{
    public List<EngineeringSystem> GetSystems(string filter);
}