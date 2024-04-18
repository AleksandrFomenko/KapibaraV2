﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using KapibaraV2.Helpers;

namespace Kapibara2.Views.MepGeneral
{
    public partial class SystemNameView : Window
    {
        public SystemNameView(Document doc)
        {
            InitializeComponent();
            Doc = doc;
        }
        Document Doc;
        private List<BuiltInCategory> cats_duct = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_DuctCurves,
            BuiltInCategory.OST_FlexDuctCurves,
            BuiltInCategory.OST_DuctInsulations,
            BuiltInCategory.OST_DuctLinings,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_DuctTerminal,
            BuiltInCategory.OST_DuctFitting
        };

        private List<BuiltInCategory> cats_pipes = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_PipeCurves,
            BuiltInCategory.OST_PipeInsulations,
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_PipeFitting,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_Sprinklers,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_FlexPipeCurves
        };

        private List<Element> elements = new List<Element>();
        private string ParameterName;
        private bool bpTypeSystem;
        private BuiltInParameter bp;
        private bool duct;
        private bool activeView;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlockSystemName.Items.Add("Имя системы");
            BlockSystemName.Items.Add("Сокращение для системы");
            BlockSystemName.Items.Add("Тип системы");
            BlockSystemName.SelectedIndex = 0;
            BlockElements.Items.Add("Трубопроводам");
            BlockElements.Items.Add("Воздуховодам");
            BlockElements.SelectedIndex = 0;

            FilteredElementCollector collector_duct = new FilteredElementCollector(Doc);

            List<Element> elements_duct = (List<Element>)collector_duct
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType().ToElements();

            FilteredElementCollector collector_pipe = new FilteredElementCollector(Doc);

            List<Element> elements_pipe = (List<Element>)collector_pipe
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType().ToElements();
            List<Parameter> allParameters = new List<Parameter>();
            if (elements_duct.Count != 0)
            {
                List<Parameter> parameters_duct = elements_duct[0].Parameters
                .Cast<Parameter>()
                .Where(parameter => parameter.StorageType == StorageType.String && parameter.Id.IntegerValue > 0)
                .Select(Parameter => Parameter)
                .ToList();
                allParameters.AddRange(parameters_duct);
            }
            if (elements_pipe.Count != 0)
            {

                List<Parameter> parameters_pipes = elements_pipe[0].Parameters
                .Cast<Parameter>()
                .Where(parameter => parameter.StorageType == StorageType.String && parameter.Id.IntegerValue > 0)
                .Select(Parameter => Parameter)
                .ToList();
                allParameters.AddRange(parameters_pipes);
            }





            foreach (Parameter par in allParameters)
            {
                if (!BlockUserParameters.Items.Contains(par.Definition.Name))
                {
                    BlockUserParameters.Items.Add(par.Definition.Name);
                }

                if (par.Definition.Name == "ААА_Имя системы")
                {
                    BlockUserParameters.SelectedItem = "ААА_Имя системы";
                }
            }
        }
        private void BlockUserParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlockUserParameters.SelectedItem != null)
            {
                string selectedParameterName = BlockUserParameters.SelectedItem.ToString();
                ParameterName = selectedParameterName;
            }
        }
        private void BlockElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedElement = BlockElements.SelectedItem.ToString();
            if (selectedElement == "Трубопроводам")
            {
                duct = false;
            }
            else if (selectedElement == "Воздуховодам")
            {
                duct = true;
            }
        }
        private void BlockSystemName_Selection(object sender, SelectionChangedEventArgs e)
        {
            string selectedElement = BlockSystemName.SelectedItem.ToString();
            if (selectedElement == "Имя системы")
            {
                bp = BuiltInParameter.RBS_SYSTEM_NAME_PARAM;
            }
            else if (selectedElement == "Сокращение для системы")
            {
                bp = BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM;
            }
            else if (selectedElement == "Тип системы")
            {
                bpTypeSystem = true;

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            activeView = true;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            activeView = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (Transaction t = new Transaction(Doc, "Start"))
            {
                t.Start();
                ExecuteTransactionSystemName();
                t.Commit();
            }

            Autodesk.Revit.UI.TaskDialog.Show("Succeeded", "Успешно");
            Close();
        }
        private string getSystemType(Element elem)
        {
            string result = "";
            if (elem is FamilyInstance fi)
            {
                List<String> nameList = new List<String>();
                MEPModel mp = fi.MEPModel;
                if (mp != null && mp.ConnectorManager != null && mp.ConnectorManager.Connectors != null)
                {
                    foreach (Connector connector in mp.ConnectorManager.Connectors)
                    {
                        if (connector.MEPSystem != null)
                        {
                            if (connector.MEPSystem is MEPSystem ms)
                            {
                                var msType = Doc.GetElement(ms.GetTypeId());
                                if (msType != null)
                                {
                                    if (bp == BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM)
                                    {
                                        nameList.Add(msType.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM).AsString());
                                    }
                                    else if (bpTypeSystem)
                                    {
                                        nameList.Add(msType.Name);
                                    }
                                }
                            }
                        }
                    }
                    nameList.Sort();
                    if (nameList.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string StringInfo in nameList)
                        {
                            sb.Append(StringInfo);
                            sb.Append(",");
                        }
                        sb.Length--;
                        result = sb.ToString();
                    }
                    nameList.Clear();
                }
            }
            return result;
        }

        public void ExecuteTransactionSystemName()
        {
            Helper helper = new Helper();
            FilteredElementCollector collector;
            if (activeView)
            {
                collector = new FilteredElementCollector(Doc, Doc.ActiveView.Id);
            }
            else
            {
                collector = new FilteredElementCollector(Doc);
            }
            if (duct)
            {
                var catFilt = new ElementMulticategoryFilter(cats_duct);
                elements = (List<Element>)collector
                    .WherePasses(catFilt)
                    .WhereElementIsNotElementType()
                    .ToElements();
            }
            else
            {
                var catFilt = new ElementMulticategoryFilter(cats_pipes);
                elements = (List<Element>)collector
                    .WherePasses(catFilt)
                    .WhereElementIsNotElementType()
                    .ToElements();
            }
            if (duct && bpTypeSystem)
            {
                bp = BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM;
            }
            else if (!duct && bpTypeSystem)
            {
                bp = BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM;
            }

            foreach (Element elem in elements)
            {
                string getSystemTypeResult = "";



                var par = elem.get_Parameter(bp);
                if (par != null)
                {
                    if (par.StorageType == StorageType.ElementId)
                    {
                        if (par.AsValueString() != null && par.AsValueString() != "Не определено")
                        {

                            helper.setParameterValueByNameToElement(elem, ParameterName, par.AsValueString());

                            foreach (Element subelem in helper.GetSubComponents(elem))
                            {
                                helper.setParameterValueByNameToElement(subelem, ParameterName, par.AsValueString());
                                foreach (Element subelem_second in helper.GetSubComponents(subelem))
                                {
                                    helper.setParameterValueByNameToElement(subelem_second, ParameterName, elem.get_Parameter(bp).AsValueString());
                                }
                            }
                        }
                        else
                        {
                            if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                            {
                                getSystemTypeResult = getSystemType(notSuperComponent);
                                helper.setParameterValueByNameToElement(notSuperComponent, ParameterName, getSystemTypeResult);
                                foreach (Element subelem in helper.GetSubComponents(notSuperComponent))
                                {
                                    helper.setParameterValueByNameToElement(subelem, ParameterName, getSystemTypeResult);
                                    foreach (Element subelem_second in helper.GetSubComponents(subelem))
                                    {
                                        helper.setParameterValueByNameToElement(subelem_second, ParameterName, getSystemTypeResult);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (par.AsString() != null && par.AsString() != "")
                        {
                            helper.setParameterValueByNameToElement(elem, ParameterName, par.AsString());
                            foreach (Element subelem in helper.GetSubComponents(elem))
                            {
                                helper.setParameterValueByNameToElement(subelem, ParameterName, par.AsString());
                                foreach (Element subelem_second in helper.GetSubComponents(subelem))
                                {
                                    helper.setParameterValueByNameToElement(subelem_second, ParameterName, par.AsString());
                                }
                            }
                        }
                        else
                        {
                            if (elem is FamilyInstance notSuperComponent && notSuperComponent.SuperComponent == null)
                            {
                                getSystemTypeResult = getSystemType(notSuperComponent);
                                helper.setParameterValueByNameToElement(notSuperComponent, ParameterName, getSystemTypeResult);
                                foreach (Element subelem in helper.GetSubComponents(notSuperComponent))
                                {
                                    helper.setParameterValueByNameToElement(subelem, ParameterName, getSystemTypeResult);
                                    foreach (Element subelem_second in helper.GetSubComponents(subelem))
                                    {
                                        helper.setParameterValueByNameToElement(subelem_second, ParameterName, getSystemTypeResult);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
