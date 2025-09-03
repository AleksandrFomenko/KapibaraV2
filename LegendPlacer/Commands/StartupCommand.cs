﻿using Autodesk.Revit.Attributes;
using JetBrains.Annotations;
using Nice3point.Revit.Toolkit.External;


namespace LegendPlacer.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    public override void Execute()
    {
       Host.Start();
    }
}