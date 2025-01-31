using System.Collections.ObjectModel;
using System.Windows;
using ExporterModels.Models.Configuration;
using ExporterModels.Models.Entities;
using ExporterModels.ProjectControl.AddProject.View;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace ExporterModels.ProjectControl.ViewModels;

internal partial class ProjectControlViewModel : ObservableObject
{
    [ObservableProperty]
    private string _listProjectsText;

    [ObservableProperty]
    private string _configFilePath;
    
    [ObservableProperty]
    private List<Project> _projects;
    
    [ObservableProperty]
    private Project _selectedProject;
    
    internal static Action UpdateProjects;

    internal ProjectControlViewModel()
    {
        ListProjectsText = "Список проектов";
        ConfigFilePath = Config.GetConfigPath();
        UpdateProjects = LoadProjects;
        UpdateProjects();
    }
    private void LoadProjects()
    {
        var projectsList = Config.GetProjects();
        Projects = new List<Project>(projectsList);
    }

    [RelayCommand]
    private void SetConfigFilePath(Window window)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            ConfigFilePath = openFileDialog.FileName;
            Config.UpdateConfigPath(ConfigFilePath);
            
        }
    }

    [RelayCommand]
    private void AddProject(Window window)
    {
        var view = new AddProjectView();
        view.ShowDialog();
    }
    
    partial void OnSelectedProjectChanged(Project value)
    {
        DeleteProjectCommand.NotifyCanExecuteChanged();
    }
    private bool CanDeleteCommand()
    {
        return (SelectedProject != null);
    }
    [RelayCommand(CanExecute = nameof(CanDeleteCommand))]
    private void DeleteProject(Window window)
    {
        if (SelectedProject != null)
        {
            Config.RemoveProject(SelectedProject.Name);
            UpdateProjects();
        }
    }
}