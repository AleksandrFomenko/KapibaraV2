using System.Windows;
using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;

namespace KapibaraV2.ViewModels.BIM.AddDeleteProjects
{
    public partial class AddProjectViewModel : ObservableObject
    {
        private readonly ExportModelsViewModel _mainViewModel;

        [ObservableProperty]
        private string _projectName;


        public AddProjectViewModel(ExportModelsViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }


        [RelayCommand]
        private void SaveProject(Window window)
        {
            var newProject = new Project
            {
                Name = ProjectName,
                Paths = new List<ModelPathForList>()
            };

            Config.AddProject(newProject);
            _mainViewModel.LoadProjects();
            window?.Close();
        }
        

        [RelayCommand]
        private void CloseWindow(Window window)
        {
            window?.Close();

        }

    }
}
