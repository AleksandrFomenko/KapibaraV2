using System.Collections;
using Nice3point.Revit.Toolkit.External.Handlers;

namespace Insolation.Server.Buffers;

public static partial class RevitShell
{
    private static ActionEventHandler? _actionEventHandler ;
    private static AsyncEventHandler? _asyncEventHandler;
    private static AsyncEventHandler<IEnumerable>? _asyncCollectionHandler;

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

    public static AsyncEventHandler<IEnumerable> AsyncCollectionHandler
    {
        get => _asyncCollectionHandler ?? throw new InvalidOperationException("The Handler was never set.");
        private set => _asyncCollectionHandler = value;
    }

    public static void RegisterHandlers()
    {
        ActionEventHandler = new ActionEventHandler();
        AsyncEventHandler = new AsyncEventHandler();
        AsyncCollectionHandler = new AsyncEventHandler<IEnumerable>();
    }
}