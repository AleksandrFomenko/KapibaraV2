using EngineeringSystems.ViewModels;
using Options = EngineeringSystems.ViewModels.Options;

namespace EngineeringSystems.Model;

public interface IEngineeringSystemsModel
{
    public void Execute(
        List<string> systemNames,
        string parametersUser,
        bool flag1,
        bool flag2,
        Options options,
        FilterOption filterOption);
    public List<string> GetUserParameters();
}