using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace SolidIntersection.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class SolidIntersection : ExternalCommand
{ 
    public override void Execute()
    {
        Host.Start();
    }
}