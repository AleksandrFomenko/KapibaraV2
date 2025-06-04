using System.Windows.Media;
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
            
            //BIM
            panelBim.AddPushButton<ExportModels>("Export\nmodels")
                .SetImage("/KapibaraV2;component/Resources/Icons/ExportModels.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ExportModels.png");

            panelBim.AddPushButton<FamilyCleaner.Commands.StartupCommand>("Cleaning\nFamily")
                .SetImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/FamilyManager.png");
            panelBim.AddPushButton<ClashDetective.Commands.StartupCommand>("Clash\nNavigator")
                .SetImage("/KapibaraV2;component/Resources/Icons/ClashNavigator.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ClashNavigator.png");
            
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
            panelMepGeneral.AddPushButton<Insolation.Commands.StartupCommand>("ываываыва")
                .SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SystemName.png");

            // Разное
            panelInfo.AddPushButton<ChatGPT.Commands.ChatGpt>("ChatGPT")
                .SetImage("/KapibaraV2;component/Resources/Icons/ai.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ai.png");
        }
        private static void ApplyResources()
        {
            ThemeWatcherService.Initialize();
            ThemeWatcherService.ApplyTheme(ApplicationTheme.Dark);
       
        }
    }
}