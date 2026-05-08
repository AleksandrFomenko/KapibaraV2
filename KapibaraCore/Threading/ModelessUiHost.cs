#nullable enable
using System.Windows;
using System.Windows.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace KapibaraCore.Threading;

public sealed class ModelessUiHost : IDisposable
{
    private static readonly Dispatcher Dispatcher;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly object _lock = new();
    private IUiSession? _session;

    public bool IsRunning => _session is { IsAlive: true };

    public ModelessUiHost(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    static ModelessUiHost()
    {
        using var ready = new ManualResetEventSlim(false);
        var uiThread = new Thread(() =>
        {
            _ = Dispatcher.CurrentDispatcher;
            ready.Set();
            Dispatcher.Run();
        });
        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.IsBackground = true;
        uiThread.Start();
        ready.Wait();
        Dispatcher = Dispatcher.FromThread(uiThread)!;
    }

    public void Start(Func<IServiceProvider, Window> windowFactory)
    {
        lock (_lock)
        {
            if (IsRunning) return;
            _session = Dispatcher.CheckAccess()
                ? new UiSession(_scopeFactory, windowFactory)
                : Dispatcher.Invoke(() => (IUiSession)new UiSession(_scopeFactory, windowFactory));
        }
    }

    public void Shutdown()
    {
        lock (_lock)
        {
            if (_session is not { IsAlive: true }) return;
            Dispatcher.Invoke(() => _session.Close());
            _session = null;
        }
    }

    public void Dispose() => Shutdown();

    private interface IUiSession
    {
        bool IsAlive { get; }
        void Close();
    }

    private sealed class UiSession : IUiSession
    {
        private readonly IServiceScope _scope;
        private readonly Window _host;
        public bool IsAlive { get; private set; } = true;

        public UiSession(IServiceScopeFactory scopeFactory, Func<IServiceProvider, Window> windowFactory)
        {
            _scope = scopeFactory.CreateScope();
            _host = windowFactory(_scope.ServiceProvider);
            _host.Closed += OnClosed;
            _host.Show();
        }

        public void Close() => _host.Close();

        private void OnClosed(object? sender, EventArgs e)
        {
            IsAlive = false;
            _scope.Dispose();
        }
    }
}