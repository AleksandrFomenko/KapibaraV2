using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KapibaraV2.ViewModels.BIM
{
    public partial class ExportModelsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string configFilePath;

        public ExportModelsViewModel()
        {
            SelectConfigFileCommand = new RelayCommand(SelectConfigFile);
            // InitializeParameters();
        }

        public ICommand SelectConfigFileCommand { get; }

        private void SelectConfigFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ConfigFilePath = openFileDialog.FileName;
            }
        }
    }
}
