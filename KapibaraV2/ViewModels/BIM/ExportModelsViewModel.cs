using System.Collections.ObjectModel;
using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;
using KapibaraV2.Views.BIM;
using KapibaraV2.ViewModels.BIM.AddDeleteProjects;
using KapibaraV2.ViewModels.BIM.ExportModels;
using KapibaraV2.Views.BIM.ExportModels;
using System.Windows;
using KapibaraV2.Models.BIM.ExportModels.Exporters;
using KapibaraV2.Models.BIM.ExportModels.Exporters.IFC;
using KapibaraV2.Models.BIM.ExportModels.Exporters.NWC;
using KapibaraV2.Models.BIM.ExportModels.Exporters.Resave;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace KapibaraV2.ViewModels.BIM
{
    public partial class ExportModelsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string configFilePath;

        [ObservableProperty]
        private ObservableCollection<Project> projects;

        [ObservableProperty]
        private Project selectedProject;

        [ObservableProperty]
        private ObservableCollection<ModelPathForList> modelPaths;

        [ObservableProperty]
        private string savePath;

        [ObservableProperty]
        private ModelPathForList selectedModelPath;

        [ObservableProperty]
        private bool isAutoMoverEnabled;

        [ObservableProperty]
        private string selectedExportOption;

        [ObservableProperty]
        private string badWorksetName;

        [ObservableProperty]
        private string ifcPath;

        public bool CanAddModel => SelectedProject != null;

        public ExportModelsViewModel()
        {
            LoadConfigFilePath();
            LoadProjects();
        }

        [RelayCommand]
        private void SelectConfigFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ConfigFilePath = openFileDialog.FileName;
                Config.UpdateConfigPath(ConfigFilePath);
                LoadProjects();
            }
        }

        [RelayCommand]
        private void SelectIFCConfigFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                IfcPath = openFileDialog.FileName;

                if (SelectedProject != null)
                {
                    SelectedProject.IfcConfigPath = IfcPath;
                    Config.SaveProject(SelectedProject);
                }
            }
        }

        [RelayCommand]
        private void AddProject()
        {
            var vmAddProject = new AddProjectViewModel(this);
            var view = new AddProjectView(vmAddProject);
            view.ShowDialog();
        }

        [RelayCommand]
        private void DeleteProject()
        {
            if (SelectedProject != null)
            {
                Config.RemoveProject(SelectedProject.Name);
                LoadProjects();
            }
        }

        [RelayCommand]
        private void SelectSavePath()
        {
            if (SelectedProject == null)
                return;

            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Выберите папку для сохранения",
                UseDescriptionForTitle = true
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string folderPath = dialog.SelectedPath;
                SelectedProject.SavePath = folderPath;
                SavePath = folderPath;
                Config.SaveProject(SelectedProject);
            }
        }

        private void LoadConfigFilePath()
        {
            ConfigFilePath = Config.GetConfigPath();
        }

        private List<string> GetCheckedModelPaths()
        {
            return ModelPaths.Where(mp => mp.IsChecked).Select(mp => mp.Path).ToList();
        }

        private void LoadSavePath()
        {
            SavePath = SelectedProject?.SavePath;
        }

        private void LoadBadWorksetName()
        {
            BadWorksetName = SelectedProject?.badNameWorkset;
        }

        public void LoadProjects()
        {
            var projectsList = Config.GetProjects();
            Projects = new ObservableCollection<Project>(projectsList);
        }

        private void LoadIfcPath()
        {
            IfcPath = SelectedProject?.IfcConfigPath;
        }

        partial void OnSelectedProjectChanged(Project value)
        {
            LoadModelPaths();
            LoadSavePath();
            LoadBadWorksetName();
            LoadIfcPath();
        }

        public void LoadModelPaths()
        {
            if (SelectedProject?.Paths != null)
            {
                ModelPaths = new ObservableCollection<ModelPathForList>(SelectedProject.Paths);
            }
            else
            {
                ModelPaths = new ObservableCollection<ModelPathForList>();
            }
        }

        [RelayCommand]
        private void AddModel()
        {
            var addModelsViewModel = new AddModelsViewModel(this);
            var addModelsView = new AddModelsView(addModelsViewModel);
            addModelsView.ShowDialog();
        }

        [RelayCommand]
        private void DeletePath()
        {
            if (SelectedProject != null && SelectedModelPath != null)
            {
                SelectedProject.Paths.Remove(SelectedModelPath);
                Config.SaveProject(SelectedProject);
                LoadModelPaths();
            }
        }

        [RelayCommand]
        private void SaveBadNameWorkset()
        {
            if (SelectedProject != null)
            {
                SelectedProject.badNameWorkset = BadWorksetName;
                Config.SaveProject(SelectedProject);
            }
        }

        [RelayCommand]
        private void Export(Window window)
        {
            
            IExporter iexporter = null;
            var checkedModelPaths = GetCheckedModelPaths();
            if (SelectedProject == null || checkedModelPaths == null || !checkedModelPaths.Any() || SelectedProject.SavePath == null)
            {
                TaskDialog.Show("PathOrSavePathIsNull", "Проект для экспорта не выбран или нет выбранных путей");
                return;
            }
            switch (SelectedExportOption)
            {
                case "Navisworks":
                    iexporter = new ExportToNwc(checkedModelPaths, SelectedProject.SavePath,
                        SelectedProject.badNameWorkset);
                    break;

                case "Пересохранить модель":
                    iexporter = new ResaveModel(checkedModelPaths, SelectedProject.SavePath,
                        SelectedProject.badNameWorkset);
                    break;

                case "Отсоединенная модель":
                    iexporter = new SaveAsCentral(checkedModelPaths, SelectedProject.SavePath,
                        SelectedProject.badNameWorkset);
                    break;

                case "IFC":
                    iexporter = new ExportToIfc(checkedModelPaths, SelectedProject.SavePath,
                        SelectedProject.badNameWorkset, SelectedProject.IfcConfigPath);
                    break;

                default:
                    TaskDialog.Show("ExportOptionNotSelected", "Опция экспорта не выбрана");
                    break;
            }
            ExportManager exportManager = new ExportManager(iexporter);
            exportManager.ExecuteExport();
            
            window?.Close();
        }
    }
}
