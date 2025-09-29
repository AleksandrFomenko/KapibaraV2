using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace ProjectAxes.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class AxesStartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Handler.Handler.RegisterHandlers();
        Host.StartAxes();
    }
}

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class LevelsStartupCommand : ExternalCommand
{
    public override void Execute()
    {
        Handler.Handler.RegisterHandlers();
        Host.StartLevels();
    }
}