using Nice3point.Revit.Toolkit.External.Handlers;

// ReSharper disable once CheckNamespace
namespace Handler;

public static class Handler
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