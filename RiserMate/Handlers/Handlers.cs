using Nice3point.Revit.Toolkit.External.Handlers;

namespace RiserMate.Handlers;

public static class Handlers
{
    private static AsyncEventHandler? _asyncEventHandler;
    public static AsyncEventHandler AsyncEventHandler
    {
        get => _asyncEventHandler ?? throw new InvalidOperationException("The Handler was never set.");
        private set => _asyncEventHandler = value;
    }

    public static void RegisterHandlers()
    {
        AsyncEventHandler = new AsyncEventHandler();
    }
}