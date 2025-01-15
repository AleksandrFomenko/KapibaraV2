using System.Windows.Media;
using Autodesk.Windows;
using Nice3point.Revit.Toolkit.External;
using KapibaraV2.Commands.MepGeneral;
using KapibaraV2.Commands.BIM;



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
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panelBim = Application.CreatePanel("BIM", "Kapibara");
            var panelGeneral = Application.CreatePanel("Общие", "Kapibara");
            var panelMepGeneral = Application.CreatePanel("MEP", "Kapibara");
            var panelInfo = Application.CreatePanel("Разное", "Kapibara");

            //BIM
            
            panelBim.AddPushButton<ExportModels>("Export\nmodels")
                .SetImage("/KapibaraV2;component/Resources/Icons/ExportModels.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ExportModels.png");

            panelBim.AddPushButton<FamilyCleaner.Commands.StartupCommand>("Cleaning\nFamily")
                .SetImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png");
            
            //panelBim.AddPushButton<FsmModules.Command.StartupCommand>("Prefab")
                //.SetImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png")
                //.SetLargeImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png");
            
            //General
            panelGeneral.AddPushButton<ImportExcelByParameter.Commands.StartupCommand>("Import\nExcel")
                .SetImage("/KapibaraV2;component/Resources/Icons/ImportExcel.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ImportExcel.png");
            //panelGeneral.AddPushButton<FloorFillerCommand>("Этаж")
               // .SetImage("/KapibaraV2;component/Resources/Icons/Floor.png")
                //.SetLargeImage("/KapibaraV2;component/Resources/Icons/Floor.png");
            panelGeneral.AddPushButton<ViewManager.Commands.StartupCommand>("View\nManager")
                .SetImage("/KapibaraV2;component/Resources/Icons/ViewManager.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ViewManager.png");
            
            var stackPanel = panelGeneral.AddStackPanel();
            stackPanel.AddLabel("Items:");
            stackPanel.AddPushButton<SolidIntersector.Commands.SolidIntersector>("Intersector")
                .SetImage("/KapibaraV2;component/Resources/Icons/intersector.png");
            stackPanel.AddPushButton<ActiveView.Commands.StartupCommand>("Active view")
                .SetImage("/KapibaraV2;component/Resources/Icons/ActiveView.png");
            //stackPanel.AddPushButton<ColorsByParameters.Commands.StartupCommand>("Цвета")
                //.SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png");
            

            //MEP общие
            panelMepGeneral.AddPushButton<SystemName.Commands.StartupCommandSystemName>("System\nname")
                .SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SystemName.png");

            // Разное
            panelInfo.AddPushButton<ChatGPT.Commands.ChatGpt>("ChatGPT")
                .SetImage("/KapibaraV2;component/Resources/Icons/ai.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ai.png");


            var ribbon = ComponentManager.Ribbon;
            var panelBackgroundBrushPurple =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 255));
            var panelBackgroundBrushLightCoral =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 128, 128));
            var panelBackgroundBrushTurquoise =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 149, 237));
            var panelBackgroundBrushPaleTurquoise =
                new SolidColorBrush(System.Windows.Media.Color.FromRgb(175, 238, 238));

            foreach (RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Title == "Kapibara")
                {
                    foreach (var panel in tab.Panels)
                    {
                        panel.CustomPanelTitleBarBackground = panel.Source.Title switch
                        {
                            "MEP" => panelBackgroundBrushPurple,
                            "Общие" => panelBackgroundBrushTurquoise,
                            "BIM" => panelBackgroundBrushLightCoral,
                            "Разное" => panelBackgroundBrushPaleTurquoise,
                            _ => panel.CustomPanelTitleBarBackground
                        };
                    }
                }
            }
        }
    }
}