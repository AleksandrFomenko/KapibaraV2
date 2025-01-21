using System.Windows;
using LevelByFloor.Models;

namespace LevelByFloor.ViewModels;

public partial class LevelByFloorViewModel : ObservableObject
{
    private Document _doc;
    private readonly LevelByFloorModel _model;
    internal static Action Close;
    
    [ObservableProperty]
    private List<string> _parameters;
    [ObservableProperty]
    private string _parameter;
    [ObservableProperty]
    private string _prefix;
    [ObservableProperty]
    private string _suffix;

    internal LevelByFloorViewModel(Document doc, LevelByFloorModel model)
    {
        _doc = doc;
        _model = model;
        Parameters = _model.LoadParameters();
    }
    partial void OnParameterChanged(string value)
    {
        ExecuteCommand.NotifyCanExecuteChanged();
    }
    private bool CanExecuteCommand()
    {
        return Parameter != null;
    }
    
    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    private void Execute(Window window)
    {
        _model.Execute();
        Close();
    }
}