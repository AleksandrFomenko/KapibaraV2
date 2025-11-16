using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace KapibaraV2.CustomPanel
{
    public static class RibbonFullGradient
    {
        public static void Apply(string tabTitle)
        {
            var ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;

            var tab = ribbon.Tabs.FirstOrDefault(t => t.Title == tabTitle);
            if (tab == null) return;


            var creamy     = (SolidColorBrush)new BrushConverter().ConvertFrom("#FAEBD7");
            var tabHeaderBg= (SolidColorBrush)new BrushConverter().ConvertFrom("#FAEBD7");
            var textBrush  = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFF"); 
            var transparent= Brushes.Transparent;

            creamy.Freeze(); textBrush.Freeze(); tabHeaderBg.Freeze();
            
            typeof(RibbonTab).GetProperty("IsContextualTab", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(tab, true);
            typeof(RibbonTab).GetProperty("IsMergedContextualTab", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(tab, false);
            typeof(RibbonTab).GetProperty("Theme", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(tab, null);

            var rc = typeof(RibbonTab).GetProperty("RibbonControl",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?.GetValue(tab) as RibbonControl;

            typeof(RibbonControl).GetMethod("SetContextualTabTheme", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(rc, new object[] { tab });
            
            var adwAsm = tab.GetType().Assembly;
            var tabThemeType = adwAsm.GetType("Autodesk.Internal.Windows.TabTheme");
            if (tabThemeType == null) return;

            var customTheme = System.Activator.CreateInstance(tabThemeType);
            if (customTheme == null) return;

            void SetThemeBrush(string propName, Brush brush)
            {
                var p = tabThemeType.GetProperty(propName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && p.CanWrite && p.PropertyType.IsAssignableFrom(typeof(Brush)))
                { p.SetValue(customTheme, brush); return; }

                var dpField = tabThemeType.GetField(propName + "Property",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (dpField?.GetValue(null) is DependencyProperty dp)
                {
                    var setValue = tabThemeType.GetMethod("SetValue",
                        BindingFlags.Instance | BindingFlags.Public, null,
                        new[] { typeof(DependencyProperty), typeof(object) }, null);
                    setValue?.Invoke(customTheme, new object[] { dp, brush });
                }
            }
            
            SetThemeBrush("TabHeaderBackground",         tabHeaderBg);
            SetThemeBrush("TabHeaderForeground",         textBrush);
            SetThemeBrush("SelectedTabHeaderBackground", tabHeaderBg);
            SetThemeBrush("SelectedTabHeaderForeground", textBrush);
            SetThemeBrush("SelectedTabHeaderBorder",     transparent);
            SetThemeBrush("RolloverTabHeaderBackground", tabHeaderBg);
            SetThemeBrush("RolloverTabHeaderForeground", textBrush);
            SetThemeBrush("RolloverTabHeaderBorder",     transparent);
            
            SetThemeBrush("PanelContentBackground",      creamy);
            SetThemeBrush("PanelBackground",             creamy);
            SetThemeBrush("PanelBackgroundVerticalLeft", creamy);
            SetThemeBrush("PanelBackgroundVerticalRight",creamy);
            SetThemeBrush("PanelTitleBackground",        creamy);
            SetThemeBrush("PanelTitleForeground",        textBrush);
            SetThemeBrush("PanelBorder",                 transparent);
            SetThemeBrush("PanelSeparatorBrush",         transparent);
            SetThemeBrush("OuterBorder",                 transparent);
            SetThemeBrush("InnerBorder",                 transparent);
            
            typeof(RibbonTab).GetProperty("Theme",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(tab, customTheme);
            
            foreach (var awPanel in tab.Panels)
            {
                SetPropOrField(awPanel, "CustomPanelBackground",         "mCustomPanelBackground",         transparent);
                SetPropOrField(awPanel, "CustomPanelTitleBarBackground", "mCustomPanelTitleBarBackground", transparent);
                SetPropOrField(awPanel, "CustomSlideOutPanelBackground", "mCustomSlideOutPanelBackground", transparent);
                SetPropOrField(awPanel, "PanelTitleForeground",          "mPanelTitleForeground",          textBrush);
            }
            
            rc?.InvalidateVisual();
            rc?.UpdateLayout();
            ComponentManager.Ribbon?.InvalidateVisual();
            ComponentManager.Ribbon?.UpdateLayout();
        }

        private static void SetPropOrField<T>(object obj, string propName, string fieldName, T value)
        {
            var t = obj.GetType();
            var p = t.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.CanWrite) { p.SetValue(obj, value); return; }
            var f = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            f?.SetValue(obj, value);
        }
    }
}
