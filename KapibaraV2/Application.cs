﻿using Autodesk.Revit.UI;
using KapibaraV2.Commands;
using KapibaraV2.Commands.mvvmTest;
using Nice3point.Revit.Toolkit.External;
using System.Windows.Controls.Ribbon;
using System.Windows.Media;
using Autodesk.Windows;
using KapibaraV2.Views;
using KapibaraV2.Commands.MepGeneral;
using KapibaraV2.Commands.BIM;
using KapibaraV2.Commands.Info;

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
            var panelBIM = Application.CreatePanel("BIM", "KapibaraV2");
            var panelMepGeneral = Application.CreatePanel("MEP общие", "KapibaraV2");
            //var panelVentilation = Application.CreatePanel("Вентиляция", "KapibaraV2");
            //var panelWater = Application.CreatePanel("Водоснабжение, канализация", "KapibaraV2");
            //var panelHeating = Application.CreatePanel("Отопление", "KapibaraV2");
            var panelInfo = Application.CreatePanel("Info", "KapibaraV2");

            //BIM
            panelBIM.AddPushButton<ExportModels>("Export models")
                .SetImage("/KapibaraV2;component/Resources/Icons/ActiveView.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ActiveView.png");

            //MEP общие
            panelMepGeneral.AddPushButton<SystemNameCommand>("Имя системы")
                .SetImage("/KapibaraV2;component/Resources/Icons/SystemName.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/SystemName.png");
            panelMepGeneral.AddPushButton<FloorFillerCommand>("Этаж")
                .SetImage("/KapibaraV2;component/Resources/Icons/Floor.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/Floor.png");
            /*
            panelMepGeneral.AddPushButton<KapibaraActiveView>("Set in \n active view")
                .SetImage("/KapibaraV2;component/Resources/Icons/ActiveView.png")
                .SetLargeImage("/KapibaraV2;component/Resources/Icons/ActiveView.png");
            */
            // Информация
            panelInfo.AddPushButton<KapibaraInfo>("Info");
            panelInfo.AddPushButton<ToolsKapi>("Tools");
        

            Autodesk.Windows.RibbonControl ribbon = Autodesk.Windows.ComponentManager.Ribbon;
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPurple = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 255));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPink = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 218, 221));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushLightCoral = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 128, 128));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushTurquoise = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 149, 237));
            System.Windows.Media.SolidColorBrush panelBackgroundBrushPaleTurquoise = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(175, 238, 238));
            foreach (Autodesk.Windows.RibbonTab tab in ribbon.Tabs)
            {
                foreach (Autodesk.Windows.RibbonPanel panel in tab.Panels)
                {
                    if (panel.Source.Title == "MEP общие")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushPurple;

                    }
                    if (panel.Source.Title == "Вентиляция")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushPink;
                    }
                    if (panel.Source.Title == "Водоснабжение, канализация")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushTurquoise;
                    }

                    if (panel.Source.Title == "BIM")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushLightCoral;
                    }

                    if (panel.Source.Title == "Отопление")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushLightCoral;
                    }
                    if (panel.Source.Title == "Info")
                    {
                        panel.CustomPanelTitleBarBackground = panelBackgroundBrushPaleTurquoise;
                    }
                }
            }
        }
    }
}