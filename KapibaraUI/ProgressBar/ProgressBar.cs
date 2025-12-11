using System.Windows;
using System.Windows.Threading;
using KapibaraUI.ProgressBar.view;
using KapibaraUI.ProgressBar.viewModel;


namespace KapibaraUI.ProgressBar;

public interface IProgressHandle : IProgress<(int val, string msg)>, IDisposable
{
    Task CloseAsync();
}

public static class ProgressBar
{
    public static async Task<IProgressHandle> ShowAsync(int max, string title = "Подготовка...")
    {
        var tcs = new TaskCompletionSource<IProgressHandle>();

        var uiThread = new Thread(() =>
        {
            try
            {
                var vm = new ProgressBarViewModel(max) { Message = title };
                var view = new ProgressBarView(vm)
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                var threadDispatcher = Dispatcher.CurrentDispatcher;
                
                view.Loaded += (s, e) =>
                {
                    tcs.SetResult(new ProgressHandle(view, vm, threadDispatcher));
                };
                view.Show();
               

                Dispatcher.Run();
                
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        uiThread.SetApartmentState(ApartmentState.STA);
        uiThread.IsBackground = true;
        uiThread.Start();

        return await tcs.Task;
    }
    
    private class ProgressHandle : IProgressHandle
    {
        private readonly Window _view;
        private readonly ProgressBarViewModel _vm;
        private readonly Dispatcher _dispatcher;

        public ProgressHandle(Window view, ProgressBarViewModel vm, Dispatcher dispatcher)
        {
            _view = view;
            _vm = vm;
            _dispatcher = dispatcher;
        }

        public void Report((int val, string msg) value)
        {
            _dispatcher.BeginInvoke(() =>
            {
                _vm.CurrentProgress = value.val;
                _vm.Message = value.msg;
            });
        }

        public async Task CloseAsync()
        {
            if (_dispatcher != null && !_dispatcher.HasShutdownStarted)
            {
                await _dispatcher.InvokeAsync(() =>
                {
                    _view.Close();
                    _dispatcher.InvokeShutdown();
                });
            }
        }

        public void Dispose()
        {
            _ = CloseAsync();
        }
    }
}