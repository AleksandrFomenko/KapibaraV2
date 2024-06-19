using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;
using KapibaraV2.Views.BIM;
using KapibaraV2.ViewModels.BIM.AddDeleteProjects;
using System.Windows.Forms;
using KapibaraV2.ViewModels.BIM.ExportModels;
using KapibaraV2.Views.BIM.ExportModels;

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
        private ObservableCollection<string> modelPaths;

        [ObservableProperty]
        private string savePath;

        [ObservableProperty]
        private string selectedModelPath;

        public ExportModelsViewModel()
        {
            LoadConfigFilePath();
            LoadProjects();
        }

        [RelayCommand]
        private void SelectConfigFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
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

            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedProject.SavePath = dialog.SelectedPath;
                    SavePath = dialog.SelectedPath;
                    Config.SaveProject(SelectedProject);
                }
            }
        }

        private void LoadConfigFilePath()
        {
            ConfigFilePath = Config.GetConfigPath();
        }

        private void LoadSavePath()
        {
            SavePath = SelectedProject?.SavePath;
        }

        public void LoadProjects()
        {
            var projectsList = Config.GetProjects();
            Projects = new ObservableCollection<Project>(projectsList);
        }

        partial void OnSelectedProjectChanged(Project value)
        {
            LoadModelPaths();
            LoadSavePath();
        }

        public void LoadModelPaths()
        {
            ModelPaths = new ObservableCollection<string>(SelectedProject?.Paths ?? new List<string>());
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

    }
}
