using ActiveView.ViewModels;

namespace ActiveView.Models;

public interface IModelActiveView
{
    List<string> GetParameters();
    void Execute(string parameterName, string value, bool setNotEmpty, Option option);
}