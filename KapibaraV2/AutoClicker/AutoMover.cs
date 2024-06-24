using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.AutoClicker
{
    public class AutoMover
    {
        private bool _isRunning;
        private Thread _moveThread;
        private int _interval;
        private Point[] _points;

        public struct Point
        {
            public int X;
            public int Y;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEEVENTF_MOVE = 0x0001;

        public AutoMover(int interval, Point[] points)
        {
            _interval = interval;
            _points = points;
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _moveThread = new Thread(AutoMove);
            _moveThread.IsBackground = true;
            _moveThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _moveThread.Join();
        }

        private void AutoMove()
        {
            int pointIndex = 0;
            while (_isRunning)
            {
                var point = _points[pointIndex];
                SetCursorPos(point.X, point.Y);
                mouse_event(MOUSEEVENTF_MOVE, (uint)point.X, (uint)point.Y, 0, 0);

                pointIndex = (pointIndex + 1) % _points.Length;

                Thread.Sleep(_interval);
            }
        }
    }
}
