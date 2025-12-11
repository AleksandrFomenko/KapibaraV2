using System.IO;
using System.Reflection;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace KapibaraUI.Services.Appearance;

public class ThemeWatcherServiceNonStatic: IThemeWatcherService
{
    private readonly List<FrameworkElement> _observedElements = [];

    private static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
    private static readonly string DllDirectory = Path.GetDirectoryName(DllPath);
    private const string DirectoryName = "SettingsConfig";
    private const string ConfigName = "config.json";

    private readonly string _configFilePath = Path.Combine(DllDirectory!, DirectoryName, ConfigName);

    public void Initialize()
    {
        UiApplication.Current.Resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/KapibaraUI;component/Styles/App.Resources.xaml", UriKind.Absolute)
        };

        ApplicationThemeManager.Changed += OnApplicationThemeManagerChanged;
    }

    private void OnApplicationThemeManagerChanged(ApplicationTheme currentApplicationTheme, System.Windows.Media.Color systemAccent)
    {
        foreach (var frameworkElement in _observedElements)
        {
            ApplicationThemeManager.Apply(frameworkElement);
            UpdateBackground(currentApplicationTheme);
            UpdateDictionary(frameworkElement);
        }
    }

    private static void UpdateDictionary(FrameworkElement frameworkElement)
    {
        var themedResources = frameworkElement.Resources.MergedDictionaries
            .Where(dictionary => dictionary.Source.OriginalString.Contains("WPF.UI;", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        frameworkElement.Resources.MergedDictionaries.Insert(0, UiApplication.Current.Resources.MergedDictionaries[0]);
        frameworkElement.Resources.MergedDictionaries.Insert(1, UiApplication.Current.Resources.MergedDictionaries[1]);

        foreach (var themedResource in themedResources)
            frameworkElement.Resources.MergedDictionaries.Remove(themedResource);
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
        _observedElements.Add(element);

        if (element.Resources.MergedDictionaries[0].Source.OriginalString != UiApplication.Current.Resources.MergedDictionaries[0].Source.OriginalString)
        {
            ApplicationThemeManager.Apply(element);
            UpdateDictionary(element);
        }
        UpdateBackground(ApplicationThemeManager.GetAppTheme());
    }

    private void OnWatchedElementUnloaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        _observedElements.Remove(element);
    }
    
    private void UpdateBackground(ApplicationTheme theme)
    {
        var backdrop = ReadBackdropFromConfig();
        foreach (var window in _observedElements.Select(Window.GetWindow).Distinct())
        {
            WindowBackgroundManager.UpdateBackground(window, theme, backdrop);
        }
    }
    
    public void ApplyTheme(ApplicationTheme theme)
    {
        var backdrop = ReadBackdropFromConfig();
        ApplicationThemeManager.Apply(theme, backdrop);
        UpdateBackground(theme);
    }
    
    private UiConfig LoadUiConfigOrDefault()
    {
        try
        {
            if (!File.Exists(_configFilePath))
                return new UiConfig();

            var cfg = KapibaraCore.Configuration.Configuration.LoadConfig<UiConfig>(_configFilePath);
            return cfg ?? new UiConfig();
        }
        catch
        {
            return new UiConfig();
        }
    }

    private WindowBackdropType ReadBackdropFromConfig() => LoadUiConfigOrDefault().Backdrop;

    public void SetConfigTheme()
    {
        var cfg = LoadUiConfigOrDefault();
        ApplyTheme(cfg.Theme);
    }
    
}
