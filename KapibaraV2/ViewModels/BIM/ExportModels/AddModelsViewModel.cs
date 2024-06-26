using KapibaraV2.Configuration;
using KapibaraV2.Models.BIM.ExportModels;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
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
                if (!string.IsNullOrEmpty(modelName))
                {
                    _mainViewModel.SelectedProject.Paths.Add(modelName);
                    Config.SaveProject(_mainViewModel.SelectedProject);
                }

                if (!string.IsNullOrEmpty(selectedFilePath))
                {
                    _mainViewModel.SelectedProject.Paths.Add(selectedFilePath);
                    Config.SaveProject(_mainViewModel.SelectedProject);
                }


                _mainViewModel.LoadProjects();
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
