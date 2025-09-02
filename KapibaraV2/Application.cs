using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using KapibaraUI.Services.Appearance;
using Nice3point.Revit.Toolkit.External;
using KapibaraV2.Commands.BIM;
using Wpf.Ui.Appearance;


namespace KapibaraV2
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            ApplyResources();
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            
            var panelSettings = Application.CreatePanel("Settings", "Kapibara");
            var panelBackgroundSettings =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(80, 200, 120));
            
            panelSettings.SetTitleBarBackground(panelBackgroundSettings);
            var panelBim = Application.CreatePanel("BIM", "Kapibara");
            var panelBackgroundBrushLightCoral =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 128, 128));
            panelBim.SetTitleBarBackground(panelBackgroundBrushLightCoral);
            
            
            var panelGeneral = Application.CreatePanel("Общие", "Kapibara");
            var panelBackgroundBrushTurquoise =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 149, 237));
            panelGeneral.SetTitleBarBackground(panelBackgroundBrushTurquoise);
            
            var panelBackgroundBrushPurple =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 255));
            var panelMepGeneral = Application.CreatePanel("MEP", "Kapibara");
            panelMepGeneral.SetTitleBarBackground(panelBackgroundBrushPurple);
            
            var panelBackgroundBrushPaleTurquoise =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(175, 238, 238));
            var panelInfo = Application.CreatePanel("Разное", "Kapibara");
            panelInfo.SetTitleBarBackground(panelBackgroundBrushPaleTurquoise);
            
            //Settings
            panelSettings.AddPushButton<Settings.Commands.StartupCommand>("Settings")
                .SetImage("/KapibaraV2;component/Resources/Icons/Settings32.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/Settings32.png");
            
            
            //BIM
            panelBim.AddPushButton<ExporterModels.Commands.StartupCommand>("Export\nmodels")
                .SetImage("/KapibaraV2;component/Resources/Icons/ExportModels.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ExportModels.png");

            panelBim.AddPushButton<FamilyCleaner.Commands.StartupCommand>("Cleaning\nFamily")
                .SetImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png");
            panelBim.AddPushButton<ClashHub.Commands.StartupCommand>("Clash\nNavigator")
                .SetImage("/KapibaraV2;component/Resources/Icons/ClashDetective.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ClashDetective.png");
            
            //panelBim.AddPushButton<FsmModules.Command.StartupCommand>("Prefab")
                //.SetImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png")
                //.SetLargeImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png");
                var stackPanelBim = panelBim.AddStackPanel();
                    stackPanelBim.AddPushButton<WorkSetLinkFiles.Commands.StartupCommand>("Worksets")
                        .SetImage("/KapibaraV2;component/Resources/Icons/WorksetLinkFiles.png");
            
            //General
            panelGeneral.AddPushButton<ImportExcelByParameter.Commands.StartupCommand>("Import\nExcel")
                .SetImage("/KapibaraV2;component/Resources/Icons/ImportExcel.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ImportExcel.png");
            panelGeneral.AddPushButton<ViewManager.Commands.StartupCommand>("View\nManager")
                .SetImage("/KapibaraV2;component/Resources/Icons/ViewManager.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ViewManager.png");
            panelGeneral.AddPushButton<LevelByFloor.Commands.StartupCommand>("Level\nby floor")
                .SetImage("/KapibaraV2;component/Resources/Icons/LevelByFloor.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/LevelByFloor.png");
            
            var stackPanel1 = panelGeneral.AddStackPanel();
            stackPanel1.AddPushButton<ViewByParameter.Commands.StartupCommand>("Filter view")
                .SetImage("/KapibaraV2;component/Resources/Icons/ViewByParameter.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ViewByParameter.png");
            stackPanel1.AddPushButton<LegendPlacer.Commands.StartupCommand>("Legend placer")
                .SetImage("/KapibaraV2;component/Resources/Icons/LedendPlacer.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/LedendPlacer.png");
            UpdateRibbonButton<ViewByParameter.Commands.StartupCommand>("Kapibara", "Общие");
            UpdateRibbonButton<LegendPlacer.Commands.StartupCommand>("Kapibara", "Общие");
            
            
            var stackPanel = panelGeneral.AddStackPanel();
            stackPanel.AddPushButton<SortingCategories.Commands.StartupCommand>("Sorting")
                .SetImage("/KapibaraV2;component/Resources/Icons/Sort.png");
            stackPanel.AddPushButton<SolidIntersection.Commands.SolidIntersection>("Intersection")
                .SetImage("/KapibaraV2;component/Resources/Icons/intersector.png");
            stackPanel.AddPushButton<ActiveView.Commands.StartupCommand>("Active view")
                .SetImage("/KapibaraV2;component/Resources/Icons/ActiveView.png");
            //stackPanel.AddPushButton<ColorsByParameters.Commands.StartupCommand>("Цвета")
                //.SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png");
            

            //MEP общие
            panelMepGeneral.AddPushButton<EngineeringSystems.Commands.StartupCommand>("System\nname")
                .SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SystemName.png");
            panelMepGeneral.AddPushButton<HeatingDevices.Commands.StartupCommand>("Space\nHeater")
                .SetImage("/KapibaraV2;component/Resources/Icons/SpaceHeater32.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SpaceHeater32.png");
            /*
            panelMepGeneral.AddPushButton<Insolation.Commands.StartupCommand>("ываываыва")
                .SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SystemName.png");
*/
            // Разное
            panelInfo.AddPushButton<ChatGPT.Commands.ChatGpt>("ChatGPT")
                .SetImage("/KapibaraV2;component/Resources/Icons/ai.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ai.png");
        }
        private static void ApplyResources()
        {
            ThemeWatcherService.Initialize();
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        }

        private PushButtonData CreatePushButtonData<TCommand>(string buttonText) where TCommand : IExternalCommand, new()
        {
            var command = typeof(TCommand);
            return new PushButtonData(command.FullName, buttonText, Assembly.GetAssembly(command)!.Location, command.FullName);
        }
        
        private static void UpdateRibbonButton(string tabId, string panelName, string commandId)
        {
            foreach (RibbonTab tab in Autodesk.Windows.ComponentManager.Ribbon.Tabs)
            {
                if (tab.KeyTip != null)
                    continue;

                if (tab.Id == tabId)
                {
                    foreach (var panel in tab.Panels)
                    {
                        if (panel.Source.Name == panelName)
                        {
                            foreach (object item in panel.Source.ItemsView)
                            {
                                if (item is Autodesk.Windows.RibbonButton ribbonButton &&
                                    ribbonButton.Id == $"CustomCtrl_%CustomCtrl_%{tabId}%{panelName}%{commandId}")
                                {
                                    ribbonButton.Size = RibbonItemSize.Large;
                                   
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private static void UpdateRibbonButton<TCommand>(string tabId, string panelName) where TCommand : IExternalCommand
        {
            UpdateRibbonButton(tabId, panelName, typeof(TCommand).FullName);
        }
    }
}