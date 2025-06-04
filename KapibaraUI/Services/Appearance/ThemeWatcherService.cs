using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace KapibaraUI.Services.Appearance;
    public sealed class ThemeWatcherService : IThemeWatcherService
    {
        private static readonly List<FrameworkElement> ObservedElements = [];

        
        public static void Initialize()
        {
            UiApplication.Current.Resources = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/KapibaraUI;component/Styles/App.Resources.xaml", UriKind.Absolute)
            };
            ApplicationThemeManager.Changed += OnApplicationThemeManagerChanged;
        }

        
        public static void ApplyTheme(ApplicationTheme theme)
        {
            ApplicationThemeManager.Apply(theme, WindowBackdropType.Auto);
            UpdateBackground(theme);
        }

        private static void OnApplicationThemeManagerChanged(ApplicationTheme currentApplicationTheme, System.Windows.Media.Color systemAccent)
        {
            foreach (var frameworkElement in ObservedElements)
            {
                ApplicationThemeManager.Apply(frameworkElement);
                UpdateDictionary(frameworkElement);
            }
        }

        private static void UpdateDictionary(FrameworkElement frameworkElement)
        {
            
            var themedResources = frameworkElement.Resources.MergedDictionaries
                .Where(dictionary => dictionary.Source.OriginalString.Contains("KapibaraUI;", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (UiApplication.Current.Resources.MergedDictionaries.Count >= 2)
            {
                frameworkElement.Resources.MergedDictionaries.Insert(0, UiApplication.Current.Resources.MergedDictionaries[0]);
                frameworkElement.Resources.MergedDictionaries.Insert(1, UiApplication.Current.Resources.MergedDictionaries[1]);
            }

            foreach (var themedResource in themedResources)
            {
                frameworkElement.Resources.MergedDictionaries.Remove(themedResource);
            }
        }

        public void Watch(FrameworkElement frameworkElement)
        {
            ApplicationThemeManager.Apply(frameworkElement);
            frameworkElement.Loaded += OnWatchedElementLoaded;
            frameworkElement.Unloaded += OnWatchedElementUnloaded;
        }

        private void OnWatchedElementLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ObservedElements.Add(element);

            if (element.Resources.MergedDictionaries[0].Source.OriginalString != UiApplication.Current.Resources.MergedDictionaries[0].Source.OriginalString)
            {
                ApplicationThemeManager.Apply(element);
                UpdateDictionary(element);
            }
        }

        private void OnWatchedElementUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            ObservedElements.Remove(element);
        }
        

        private static void UpdateBackground(ApplicationTheme theme)
        {
            foreach (var window in ObservedElements.Select(Window.GetWindow).Distinct())
            {
                WindowBackgroundManager.UpdateBackground(window, theme, WindowBackdropType.Mica);
            }
        }
    }
    
    
public interface IThemeWatcherService
{
    void Watch(FrameworkElement frameworkElement);
}

