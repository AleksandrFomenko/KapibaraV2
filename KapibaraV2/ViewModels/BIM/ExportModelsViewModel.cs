using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;
using KapibaraV2.Views.BIM;
using KapibaraV2.ViewModels.BIM.AddDeleteProjects;

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
        private void AddProject()
        {
            var vmAddProject = new AddProjectViewModel(this);
            var view = new  AddProjectView(vmAddProject);

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

        private void LoadConfigFilePath()
        {
            ConfigFilePath = Config.GetConfigPath();
        }

        public void LoadProjects()
        {
            var projectsList = Config.GetProjects();
            Projects = new ObservableCollection<Project>(projectsList);
        }
    }
}
