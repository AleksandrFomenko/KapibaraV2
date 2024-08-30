using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace KapibaraV2.AutoClicker
{
    internal static class NativeMethods
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);

        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
    }

    public class AutoMover
    {
        private bool _isRunning;
        private Thread _moveThread;
        private int _interval;
        private Point[] _points;
        private uint _previousExecutionState;

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
            {
                _previousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS |
                    NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
                return;
            }

          

            // Устанавливаем состояние системы, чтобы предотвратить переход в спящий режим
            _previousExecutionState = NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
            if (_previousExecutionState == 0)
            {
                TaskDialog.Show("ыва", "аа");
            }

            _isRunning = true;
            _moveThread = new Thread(AutoMove);
            _moveThread.IsBackground = true;
            _moveThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
                return;
            }
            
            _isRunning = false;
            _moveThread.Join();

            // Восстанавливаем предыдущее состояние системы
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
        }

        private void AutoMove()
        {
            int pointIndex = 0;
            while (_isRunning)
            {
                var point = _points[pointIndex];
                SetCursorPos(point.X, point.Y);
                // Обновляем состояние системы, чтобы предотвратить спящий режим
                NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);

                mouse_event(MOUSEEVENTF_MOVE, (uint)point.X, (uint)point.Y, 0, 0);

                pointIndex = (pointIndex + 1) % _points.Length;

                Thread.Sleep(_interval);
            }
        }
    }
}
