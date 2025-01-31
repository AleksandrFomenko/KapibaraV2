using System.Windows;

namespace ExporterModels.RevitModelsControl.VM;

public partial class RevitModelsControlViewModel : ObservableObject
{
    [ObservableProperty]
    private string _listModelsText;
    [ObservableProperty]
    private string _tableName;
    [ObservableProperty]
    private string _tableFullPath;

    internal RevitModelsControlViewModel()
    {
        ListModelsText = "Модели";
        TableName = "Раздел";
        TableFullPath = "Путь";
    }
    
    [RelayCommand]
    private void AddRevitModel(Window window)
    {
        
    }
}