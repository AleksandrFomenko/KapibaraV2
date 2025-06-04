using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Insolation.Server;
using Insolation.Server.Buffers;
using Nice3point.Revit.Toolkit.External;
using Insolation.ViewModels;
using Insolation.Views;
using JetBrains.Annotations;
using Nice3point.Revit.Toolkit.External.Handlers;

namespace Insolation.Commands;

/// <summary>
///     External command entry point invoked from the Revit interface
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : ExternalCommand
{
    private LineVisualization _server;
    public override void Execute()
    {
        try
        {
            _server = new LineVisualization();
 
            RevitShell.RegisterHandlers();
            var server = new LineVisualization();
            var viewModel = new InsolationViewModel();
            var view = new InsolationView(viewModel);

            view.Loaded += OnViewLoaded;

            view.Closed += OnViewClosed;

            view.Show(UiApplication.MainWindowHandle);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        var _vertices = new List<XYZ>
        {
            new XYZ(0, 0, 0),
            new XYZ(1 /304.8 * 1000, 0, 0),
        };
        var lines = new List<Line>()
        {
            Line.CreateBound(new XYZ(0, 0, 0),
                new XYZ(0, 0, 5)),
            Line.CreateBound(new XYZ(1, 0, 0),
                new XYZ(1, 0, 5)),
        };
        try
        {
            _server.Register(lines);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
           
        }
    }

    private void OnViewClosed(object sender, EventArgs e)
    {
        _server.Unregister();

    }
}