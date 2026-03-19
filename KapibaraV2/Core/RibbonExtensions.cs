using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using UIFramework;
using Color = System.Windows.Media.Color;
using ComboBox = Autodesk.Revit.UI.ComboBox;
using RibbonButton = Autodesk.Revit.UI.RibbonButton;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;
using TextBox = Autodesk.Revit.UI.TextBox;

namespace KapibaraV2.Core;

public static partial class RibbonExtensions
{

    public static RibbonPanel CreatePanel(this UIControlledApplication application, string panelName)
    {
        foreach (var panel in application.GetRibbonPanels(Tab.AddIns))
        {
            if (panel.Name == panelName)
            {
                return panel;
            }
        }

        return application.CreateRibbonPanel(panelName);
    }


    public static RibbonPanel CreatePanel(this UIControlledApplication application, string panelName, string tabName)
    {
        var cachedTabs = GetCachedTabs();
        if (cachedTabs.TryGetValue(tabName, out var cachedPanels))
        {
            if (cachedPanels.TryGetValue(panelName, out var cachedPanel))
            {
                return cachedPanel;
            }
        }

        var tabsCollection = new List<RibbonTab>();
        foreach (var tab in RevitRibbonControl.RibbonControl.Tabs)
        {
            if (tab.Id == tabName || tab.Title == tabName)
            {
                tabsCollection.Add(tab);
            }
        }

        var existedTab = tabsCollection.Count switch
        {
            1 => tabsCollection[0],
            0 => null,
            _ => tabsCollection.FirstOrDefault(tab => tab.IsVisible) ?? tabsCollection[0]
        };

        if (existedTab is not null)
        {
            var (internalPanel, panel) = CreateInternalPanel(existedTab.Id, panelName);
            existedTab.Panels.Add(internalPanel);
            return panel;
        }

        application.CreateRibbonTab(tabName);
        return application.CreateRibbonPanel(tabName, panelName);
    }

    public static void RemovePanel(this RibbonPanel panel)
    {
        var cachedPanels = GetCachedTabs();

        var internalPanel = panel.GetInternalPanel();
        var internalTab = internalPanel.Tab;

        internalTab.Panels.Remove(internalPanel);
        if (internalTab.Panels.Count == 0)
        {
            ComponentManager.Ribbon.Tabs.Remove(internalTab);
        }

        var ribbonPanels = cachedPanels[internalTab.Id];
        ribbonPanels.Remove(panel.Name);
        if (ribbonPanels.Count == 0)
        {
            cachedPanels.Remove(internalTab.Id);
        }
    }


    public static RibbonPanel SetBackground(this RibbonPanel panel, string color)
    {
        var convertedColor = (Color)ColorConverter.ConvertFromString(color);

        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelBackground = new SolidColorBrush(convertedColor);

        return panel;
    }


    public static RibbonPanel SetBackground(this RibbonPanel panel, Color color)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelBackground = new SolidColorBrush(color);

        return panel;
    }


    public static RibbonPanel SetBackground(this RibbonPanel panel, Brush brush)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelBackground = brush;

        return panel;
    }


    public static RibbonPanel SetTitleBarBackground(this RibbonPanel panel, string color)
    {
        var convertedColor = (Color)ColorConverter.ConvertFromString(color);

        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelTitleBarBackground = new SolidColorBrush(convertedColor);

        return panel;
    }


    public static RibbonPanel SetTitleBarBackground(this RibbonPanel panel, Color color)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelTitleBarBackground = new SolidColorBrush(color);

        return panel;
    }


    public static RibbonPanel SetTitleBarBackground(this RibbonPanel panel, Brush brush)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomPanelTitleBarBackground = brush;

        return panel;
    }


    public static RibbonPanel SetSlideOutPanelBackground(this RibbonPanel panel, string color)
    {
        var convertedColor = (Color)ColorConverter.ConvertFromString(color);

        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomSlideOutPanelBackground = new SolidColorBrush(convertedColor);

        return panel;
    }


    public static RibbonPanel SetSlideOutPanelBackground(this RibbonPanel panel, Color color)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomSlideOutPanelBackground = new SolidColorBrush(color);

        return panel;
    }


    public static RibbonPanel SetSlideOutPanelBackground(this RibbonPanel panel, Brush brush)
    {
        var internalPanel = GetInternalPanel(panel);
        internalPanel.CustomSlideOutPanelBackground = brush;

        return panel;
    }

    


    public static PushButton AddPushButton(this RibbonPanel panel, Type command, string buttonText)
    {
        var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command)!.Location, command.FullName);
        return (PushButton)panel.AddItem(pushButtonData);
    }


    public static PushButton AddPushButton<TCommand>(this RibbonPanel panel, string buttonText) where TCommand : IExternalCommand, new()
    {
        var command = typeof(TCommand);
        var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command)!.Location, command.FullName);
        return (PushButton)panel.AddItem(pushButtonData);
    }


    public static PushButton AddPushButton(this PulldownButton pullDownButton, Type command, string buttonText)
    {
        var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command)!.Location, command.FullName);
        return pullDownButton.AddPushButton(pushButtonData);
    }


    public static PushButton AddPushButton<TCommand>(this PulldownButton pullDownButton, string buttonText) where TCommand : IExternalCommand, new()
    {
        var command = typeof(TCommand);
        var pushButtonData = new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command)!.Location, command.FullName);
        return pullDownButton.AddPushButton(pushButtonData);
    }


    public static PulldownButton AddPullDownButton(this RibbonPanel panel, string buttonText)
    {
        return AddPullDownButton(panel, Guid.NewGuid().ToString(), buttonText);
    }


    public static PulldownButton AddPullDownButton(this RibbonPanel panel, string internalName, string buttonText)
    {
        var pushButtonData = new PulldownButtonData(internalName, buttonText);
        return (PulldownButton)panel.AddItem(pushButtonData);
    }

    public static SplitButton AddSplitButton(this RibbonPanel panel, string buttonText)
    {
        return AddSplitButton(panel, Guid.NewGuid().ToString(), buttonText);
    }


    public static SplitButton AddSplitButton(this RibbonPanel panel, string internalName, string buttonText)
    {
        var pushButtonData = new SplitButtonData(internalName, buttonText);
        return (SplitButton)panel.AddItem(pushButtonData);
    }


    public static RadioButtonGroup AddRadioButtonGroup(this RibbonPanel panel)
    {
        return AddRadioButtonGroup(panel, Guid.NewGuid().ToString());
    }


    public static RadioButtonGroup AddRadioButtonGroup(this RibbonPanel panel, string internalName)
    {
        var pushButtonData = new RadioButtonGroupData(internalName);
        return (RadioButtonGroup)panel.AddItem(pushButtonData);
    }

    public static ComboBox AddComboBox(this RibbonPanel panel)
    {
        return AddComboBox(panel, Guid.NewGuid().ToString());
    }


    public static ComboBox AddComboBox(this RibbonPanel panel, string internalName)
    {
        var pushButtonData = new ComboBoxData(internalName);
        return (ComboBox)panel.AddItem(pushButtonData);
    }


    public static TextBox AddTextBox(this RibbonPanel panel)
    {
        return AddTextBox(panel, Guid.NewGuid().ToString());
    }


    public static TextBox AddTextBox(this RibbonPanel panel, string internalName)
    {
        var pushButtonData = new TextBoxData(internalName);
        return (TextBox)panel.AddItem(pushButtonData);
    }


    public static RibbonButton SetImage(this RibbonButton button, string uri)
    {
#if REVIT2024_OR_GREATER
    if (TryGetThemedUri(uri, out var themedIconUri))
    {
        MonitorButtonTheme(button);
    }
    else
    {
        themedIconUri = uri;
    }
#else
        var themedIconUri = uri;
#endif

        button.Image = new BitmapImage(new Uri(themedIconUri, UriKind.RelativeOrAbsolute));
        return button;
    }


    public static RibbonButton SetLargeImage(this RibbonButton button, string uri)
    {
#if REVIT2024_OR_GREATER
        if (TryGetThemedUri(uri, out var themedIconUri))
        {
            MonitorButtonTheme(button);
        }
         else
        {
            themedIconUri = uri;
        }
#else
        var themedIconUri = uri;
#endif

        button.LargeImage = new BitmapImage(new Uri(themedIconUri, UriKind.RelativeOrAbsolute));
        return button;
    }


    public static PushButton SetAvailabilityController<T>(this PushButton button) where T : IExternalCommandAvailability, new()
    {
        button.AvailabilityClassName = typeof(T).FullName;
        return button;
    }
    

    public static PushButton AddShortcuts(this PushButton button, string representation)
    {
        AddButtonShortcuts(button, representation);
        return button;
    }
    

    public static PushButton AddShortcuts(this PushButton button, params IEnumerable<string> shortcuts)
    {
        AddButtonShortcuts(button, string.Join("#", shortcuts));
        return button;
    }


    public static RibbonItem SetToolTip(this RibbonItem button, string tooltip)
    {
        button.ToolTip = tooltip;
        return button;
    }
    
    public static RibbonItem SetLongDescription(this RibbonItem button, string description)
    {
        button.LongDescription = description;
        return button;
    }
    public static PushButton HideLabel(this PushButton button)
    {
        var rb = GetAdwButton(button);
        if (rb != null) SetShowText(rb, false);
        return button;
    }

    
    static RibbonButton GetAdwButton(PushButton button)
    {
        var p = typeof(PushButton).GetProperty("RibbonItem", BindingFlags.Instance | BindingFlags.NonPublic);
        return p?.GetValue(button) as RibbonButton;
    }

    static void SetShowText(RibbonButton rb, bool visible)
    {
        var p = rb.GetType().GetProperty("ShowText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null && p.CanWrite) { p.SetValue(rb, visible); return; }
        var p2 = rb.GetType().GetProperty("IsTextVisible", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p2 != null && p2.CanWrite) p2.SetValue(rb, visible);
    }
    
}