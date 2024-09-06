using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Timers;
using System.Windows;
using KapibaraV2.AutoClicker;
using Newtonsoft.Json.Linq;

namespace KapibaraV2.ViewModels.Tools
{
    public partial class ToolsKapiViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isAutoMoverEnabled;

        private AutoMover autoMover;
        private readonly System.Timers.Timer timer;
        private readonly Random random = new Random();
        private readonly int interval = 3000;
        private NativeMethods nativeMethods;

        public ToolsKapiViewModel()
        {
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += Timer_Elapsed;

            InitializeAutoMover();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsAutoMoverEnabled)
            {
                autoMover.Stop();
                InitializeAutoMover();
                autoMover.Start();
            }
        }

        private void InitializeAutoMover()
        {
            AutoMover.Point[] points = GenerateRandomPoints(5, 100, 500);
            autoMover = new AutoMover(interval, points);
        }

        private AutoMover.Point[] GenerateRandomPoints(int count, int minValue, int maxValue)
        {
            AutoMover.Point[] points = new AutoMover.Point[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = new AutoMover.Point
                {
                    X = random.Next(minValue, maxValue),
                    Y = random.Next(minValue, maxValue)
                };
            }
            return points;
        }

        partial void OnIsAutoMoverEnabledChanged(bool value)
        {
            if (value)
            {
                EnableAutoMover();
            }
            else
            {
                DisableAutoMover();
            }
        }

        private void EnableAutoMover()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
            autoMover.Start();
            timer.Start();
        }

        private void DisableAutoMover()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
            autoMover.Stop();
            timer.Stop();
        }
        public void OnWindowClosing()
        {
            DisableAutoMover();
        }

        [RelayCommand]
        private void CloseWindow(Window window)
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
            DisableAutoMover();
            window?.Close();
        }
    }
}
