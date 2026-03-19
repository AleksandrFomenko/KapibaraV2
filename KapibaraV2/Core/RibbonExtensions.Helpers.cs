using System.Reflection;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using UIFramework;
using UIFrameworkServices;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;
#if REVIT2024_OR_GREATER
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using RibbonButton = Autodesk.Revit.UI.RibbonButton;
#endif

// ReSharper disable once CheckNamespace
namespace KapibaraV2.Core;

public static partial class RibbonExtensions
{
#if REVIT2024_OR_GREATER
    /// <summary>
    ///     List of Ribbon buttons that are monitored for theme changes.
    /// </summary>
    private static HashSet<RibbonButton> _themedButtons = [];
#endif
    
    private static void AddButtonShortcuts(PushButton button, string representation)
    {
        var internalItem = GetInternalItem(button);
        ShortcutsHelper.LoadCommands();

        var shortcutItem = ShortcutsHelper.Commands[internalItem.Id];
        if (shortcutItem.ShortcutsRep is not null) return;

        shortcutItem.ShortcutsRep = representation;

        KeyboardShortcutService.applyShortcutChanges(ShortcutsHelper.Commands);
    }
    
    private static RibbonPanel CreatePanel(Autodesk.Windows.RibbonPanel panel, string tabId)
    {
        var type = typeof(RibbonPanel);
#if NETCOREAPP
        var constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            [typeof(Autodesk.Windows.RibbonPanel), typeof(string)])!;
#else
        var constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly,
            null,
            [typeof(Autodesk.Windows.RibbonPanel), typeof(string)],
            null)!;
#endif
        return (RibbonPanel)constructorInfo.Invoke([panel, tabId]);
    }


    private static (Autodesk.Windows.RibbonPanel internalPanel, RibbonPanel panel) CreateInternalPanel(string tabId, string panelName)
    {
        var internalPanel = new Autodesk.Windows.RibbonPanel
        {
            Source = new RibbonPanelSource
            {
                Title = panelName
            }
        };

        var cachedTabs = GetCachedTabs();
        if (!cachedTabs.TryGetValue(tabId, out var cachedPanels))
        {
            cachedTabs[tabId] = cachedPanels = new Dictionary<string, RibbonPanel>();
        }

        var panel = CreatePanel(internalPanel, tabId);
        panel.Name = panelName;
        cachedPanels[panelName] = panel;

        return (internalPanel, panel);
    }
    
    private static Dictionary<string, Dictionary<string, RibbonPanel>> GetCachedTabs()
    {
        var applicationType = typeof(UIApplication);
        var panelsField = applicationType.GetField("m_ItemsNameDictionary", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)!;
        return (Dictionary<string, Dictionary<string, RibbonPanel>>)panelsField.GetValue(null)!;
    }
    
    private static Autodesk.Windows.RibbonItem GetInternalItem(this RibbonItem ribbonItem)
    {
        var internalField = typeof(RibbonItem).GetField("m_RibbonItem", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        return (Autodesk.Windows.RibbonItem)internalField.GetValue(ribbonItem)!;
    }

   
    private static Autodesk.Windows.RibbonPanel GetInternalPanel(this RibbonPanel panel)
    {
        var internalField = panel.GetType().GetField("m_RibbonPanel", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        return (Autodesk.Windows.RibbonPanel)internalField.GetValue(panel)!;
    }

#if REVIT2024_OR_GREATER
    /// <summary>
    ///     Monitors the button for theme changes and updates the image accordingly.
    /// </summary>
    /// <param name="button">The Ribbon button to monitor.</param>
    private static void MonitorButtonTheme(RibbonButton button)
    {
        ApplicationTheme.CurrentTheme.PropertyChanged -= OnApplicationThemeChanged;
        ApplicationTheme.CurrentTheme.PropertyChanged += OnApplicationThemeChanged;

        _themedButtons.Add(button);
    }

    /// <summary>
    ///     Handles the ApplicationThemeChanged event and updates the button image to match the current UI theme.
    /// </summary>
    private static void OnApplicationThemeChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(ApplicationTheme.CurrentTheme.RibbonPanelBackgroundBrush)) return;
        if (UIThemeManager.CurrentTheme.ToString() == ApplicationTheme.CurrentTheme.RibbonTheme.Name) return;

        foreach (var button in _themedButtons)
        {
            if (button.Image is BitmapImage image)
            {
                TryGetThemedUri(image.UriSource.OriginalString, out var themedIconUri);
                button.Image = new BitmapImage(new Uri(themedIconUri, UriKind.RelativeOrAbsolute));
            }

            if (button.LargeImage is BitmapImage largeImage)
            {
                TryGetThemedUri(largeImage.UriSource.OriginalString, out var themedIconUri);
                button.LargeImage = new BitmapImage(new Uri(themedIconUri, UriKind.RelativeOrAbsolute));
            }
        }
    }
    
    private static bool TryGetThemedUri(string uri, out string result)
    {
        var theme = UITheme.Light;
        var themeIndex = uri.LastIndexOf("light", StringComparison.OrdinalIgnoreCase);
        if (themeIndex == -1)
        {
            theme = UITheme.Dark;
            themeIndex = uri.LastIndexOf("dark", StringComparison.OrdinalIgnoreCase);
            if (themeIndex == -1)
            {
                result = uri;
                return false;
            }
        }

        result = UIThemeManager.CurrentTheme switch
        {
            UITheme.Light when theme == UITheme.Dark => UpdateThemeUri(uri, "dark", "light", themeIndex),
            UITheme.Dark when theme == UITheme.Light => UpdateThemeUri(uri, "light", "dark", themeIndex),
            _ => uri
        };

        return true;

        static string UpdateThemeUri(string source, string currentTheme, string newTheme, int themeIndex)
        {
#if NETCOREAPP
            var sourceSpan = source.AsSpan();
            var before = sourceSpan[..themeIndex];
            var after = sourceSpan[(themeIndex + currentTheme.Length)..];
#else
            var before = source[..themeIndex];
            var after = source[(themeIndex + currentTheme.Length)..];
#endif
            return after.IndexOf(Path.AltDirectorySeparatorChar) >= 0 || after.IndexOf(Path.DirectorySeparatorChar) >= 0 ? source : string.Concat(before, newTheme, after);
        }
    }
#endif
}