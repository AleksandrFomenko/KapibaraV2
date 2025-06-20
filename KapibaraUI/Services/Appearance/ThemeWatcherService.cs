using System.IO;
using System.Reflection;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;

namespace KapibaraUI.Services.Appearance;
    public sealed class ThemeWatcherService : IThemeWatcherService
    {
        private static readonly List<FrameworkElement> ObservedElements = [];
        private ThemesDictionary _theme = new();
        private ControlsDictionary _controlsDictionary = new ();
        
        private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
        private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
        private const string DirectoryName = "SettingsConfig";
        private const string ConfigName = "config.json";
        
        private string _configFilePath = Path.Combine(DllDirectory, DirectoryName, ConfigName);
        
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
                WindowBackgroundManager.UpdateBackground(window, theme, WindowBackdropType.Auto);
            }
        }
        
        public void SetTheme(ApplicationTheme theme, FrameworkElement frameworkElement)
        {
            _theme.Theme = theme;
            ApplicationThemeManager.Apply(theme);
            if (!frameworkElement.Resources.MergedDictionaries.Contains(_theme))
            {
                frameworkElement.Resources.MergedDictionaries.Add(_theme);
                frameworkElement.Resources.MergedDictionaries.Add(_controlsDictionary);
            }

            if (frameworkElement is Window window)
            {
                WindowBackgroundManager.UpdateBackground(window, theme, WindowBackdropType.Mica);
            }
        }

        public void SetConfigTheme(FrameworkElement frameworkElement)
        {
            if (!File.Exists(_configFilePath))
            {
                SetTheme(ApplicationTheme.Light, frameworkElement);
                return;
            }
            var wrapper = KapibaraCore.Configuration.Configuration.LoadConfig<ConfigWrapper>(_configFilePath);
            var setting = wrapper?.Setting;
            if (setting == null)
            {
                SetTheme(ApplicationTheme.Light, frameworkElement);
                return;
            }
            var theme = setting.Theme == Theme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
            SetTheme(theme, frameworkElement);
        }
    }

    public interface IThemeWatcherService
{
    void Watch(FrameworkElement frameworkElement);
    void SetTheme(ApplicationTheme theme, FrameworkElement frameworkElement);
    void SetConfigTheme(FrameworkElement frameworkElement);
}

