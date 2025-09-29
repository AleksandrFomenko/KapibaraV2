namespace ProjectAxes.Models;

public interface IModel
{
    void DoAll(bool beginCheck, bool endCheck);
    void DoSelection(bool beginCheck, bool endCheck);
}