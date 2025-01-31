using System.Windows;
using ExporterModels.Models.Configuration;
using ExporterModels.Models.Entities;
using ExporterModels.ProjectControl.ViewModels;
using ModelPath = ExporterModels.Models.Entities.ModelPath;

namespace ExporterModels.ProjectControl.AddProject.ViewModel;

internal partial class AddProjectVm : ObservableObject
{
    [ObservableProperty]
    private string _projectName;
    [ObservableProperty]
    private string _heading;

    internal AddProjectVm()
    {
        Heading = "Наименование проекта";
    }

    internal static Action Close; 
    
    partial void OnProjectNameChanged(string value)
    {
        AddProjectCommand.NotifyCanExecuteChanged();
    }
    private bool CanAddCommand()
    {
        return !string.IsNullOrEmpty(ProjectName);
    }
    
    [RelayCommand(CanExecute = nameof(CanAddCommand))]
    private void AddProject(Window window)
    {
        var newProject = new Project
        {
            Name = ProjectName,
            Models = new List<ModelPath>()
        };

        Config.AddProject(newProject);
        ProjectControlViewModel.UpdateProjects();
        Close();
    }
}