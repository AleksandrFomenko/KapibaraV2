using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class AutoClicker
{
    private bool _isRunning;
    private Thread _clickThread;
    private int _interval;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    public AutoClicker(int interval)
    {
        _interval = interval;
    }

    public void Start()
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _clickThread = new Thread(AutoClick);
        _clickThread.IsBackground = true;
        _clickThread.Start();
    }

    public void Stop()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _clickThread.Join();
    }

    private void AutoClick()
    {
        while (_isRunning)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(_interval);
        }
    }
}
