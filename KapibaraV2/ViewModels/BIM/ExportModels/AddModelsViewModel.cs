using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;
using System.Windows;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.Input;

namespace KapibaraV2.ViewModels.BIM.ExportModels
{
    public partial class AddModelsViewModel : ObservableObject
    {
        private readonly ExportModelsViewModel _mainViewModel;

        [ObservableProperty]
        private string modelName;

        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private string selectedExportOption;

        public AddModelsViewModel(ExportModelsViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void SaveModel(Window window)
        {
            if (_mainViewModel.SelectedProject != null)
            {
                if (!string.IsNullOrEmpty(ModelName)) // Использование свойства, а не поля
                {
                    var modelPath = new ModelPathForList { Path = ModelName, IsChecked = false };
                    _mainViewModel.SelectedProject.Paths.Add(modelPath);
                    Config.SaveProject(_mainViewModel.SelectedProject);
                }

                if (!string.IsNullOrEmpty(SelectedFilePath)) // Использование свойства, а не поля
                {
                    var modelPath = new ModelPathForList { Path = SelectedFilePath, IsChecked = false };
                    _mainViewModel.SelectedProject.Paths.Add(modelPath);
                    Config.SaveProject(_mainViewModel.SelectedProject);
                }

                _mainViewModel.LoadModelPaths();
                window?.Close();
            }
        }

        [RelayCommand]
        private void CloseWindow(Window window)
        {
            window?.Close();
        }

        [RelayCommand]
        private void SelectLocalModel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Revit files (*.rvt)|*.rvt|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName; 
            }
        }
    }
}
