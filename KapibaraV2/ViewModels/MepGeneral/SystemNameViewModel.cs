using KapibaraV2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using Autodesk.Revit.DB.Mechanical;
using System.Xml.Linq;
using System.Windows.Documents;
using KapibaraV2.Models.MepGeneral;

namespace KapibaraV2.ViewModels.MepGeneral
{
    public class SystemNameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Action CloseAction { get; set; }

        private List<Element> elements = new List<Element>();

        private List<BuiltInCategory> cats = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_PipeCurves,
            BuiltInCategory.OST_PipeInsulations,
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_PipeFitting,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_Sprinklers,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_FlexPipeCurves,
            BuiltInCategory.OST_DuctCurves,
            BuiltInCategory.OST_FlexDuctCurves,
            BuiltInCategory.OST_DuctInsulations,
            BuiltInCategory.OST_DuctLinings,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_DuctTerminal,
            BuiltInCategory.OST_DuctFitting
        };

        public List<string> SystemParameters { get; } = new List<string> { "Имя системы", "Сокращение для системы" };
        public List<string> UserParameters { get; set; } = new List<string>();
        public List<string> Elements { get; } = new List<string> { "Трубопроводам", "Воздуховодам" };

        private string _selectedSystemParameter;
        public string SelectedSystemParameter
        {
            get { return _selectedSystemParameter; }
            set
            {
                _selectedSystemParameter = value;
                OnPropertyChanged(nameof(SelectedSystemParameter));
            }
        }

        private string _selectedUserParameter;
        public string SelectedUserParameter
        {
            get { return _selectedUserParameter; }
            set
            {
                _selectedUserParameter = value;
                OnPropertyChanged(nameof(SelectedUserParameter));
            }
        }

        private string _selectedElement;
        public string SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged(nameof(SelectedElement));
            }
        }

        private bool _isActiveView;
        public bool IsActiveView
        {
            get { return _isActiveView; }
            set
            {
                _isActiveView = value;
                OnPropertyChanged(nameof(IsActiveView));
            }
        }

        public ICommand OkCommand { get; private set; }

        public SystemNameViewModel()
        {
            InitializeParameters();
            OkCommand = new RelayCommand<object>(ExecuteOk);
        }
        private void InitializeParameters()
        {
            FilteredElementCollector collector_duct = new FilteredElementCollector(RevitApi.Document);
            FilteredElementCollector collector_pipe = new FilteredElementCollector(RevitApi.Document);


            List<Element> elements_duct = collector_duct
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .ToElements()
                .ToList();
            List<Element> elements_pipe = collector_pipe
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToElements()
                .ToList();
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
                if (!UserParameters.Contains(par.Definition.Name))
                {
                    UserParameters.Add(par.Definition.Name);
                }
            }
        }

        private void ExecuteOk(object parameter)
        {
            var catFilt = new ElementMulticategoryFilter(cats);
            FilteredElementCollector collector;
            if (_isActiveView)
            {
                collector = new FilteredElementCollector(RevitApi.Document, RevitApi.Document.ActiveView.Id);
            }
            else
            {
                collector = new FilteredElementCollector(RevitApi.Document);
            }
            elements = (List<Element>)collector
                .WherePasses(catFilt)
                .WhereElementIsNotElementType()
                .ToElements();

            using (Transaction t = new Transaction(RevitApi.Document, "SystemName"))
            {
                t.Start();
                SystemNameModel snm = new SystemNameModel(elements, _isActiveView, _selectedSystemParameter, _selectedUserParameter);
                snm.Execute();
                t.Commit(); 
            }
            Autodesk.Revit.UI.TaskDialog.Show("Succeeded", string.Format("Обработано {0} элементов", _isActiveView));
            CloseAction();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
