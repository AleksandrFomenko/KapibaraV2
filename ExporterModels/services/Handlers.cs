using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External.Handlers;

namespace ExporterModels.services;

public static class Handlers
{
    private static ActionEventHandler? _actionEventHandler;
    private static AsyncEventHandler? _asyncEventHandler;


    public static ActionEventHandler ActionEventHandler
    {
        get => _actionEventHandler ?? throw new InvalidOperationException("The Handler was never set.");
        private set => _actionEventHandler = value;
    }

    public static AsyncEventHandler AsyncEventHandler
    {
        get => _asyncEventHandler ?? throw new InvalidOperationException("The Handler was never set.");
        private set => _asyncEventHandler = value;
    }

    public static void RegisterHandlers()
    {
        ActionEventHandler = new ActionEventHandler();
        AsyncEventHandler = new AsyncEventHandler();
    }
}

public class SimpleEventHandler : IExternalEventHandler
{
    private readonly Action<UIApplication> _action;

    public SimpleEventHandler(Action<UIApplication> action)
    {
        _action = action;
    }

    public void Execute(UIApplication app)
    {
        _action(app);
    }

    public string GetName()
    {
        return "Simple External Event Handler";
    }
}