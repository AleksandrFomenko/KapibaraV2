using System.Diagnostics;
using System.Windows;
using LevelByFloor.Models;
using Options = LevelByFloor.Models.Options;

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
    private List<Options> _options;
    
    [ObservableProperty]
    private Options _option;
    
    [ObservableProperty]
    private string _prefix;
    
    [ObservableProperty]
    private string _suffix;
    
    [ObservableProperty]
    private string _indent = "0";

    internal LevelByFloorViewModel(Document doc, LevelByFloorModel model)
    {
        _doc = doc;
        _model = model;
        Parameters = _model.LoadParameters();
        Options = new List<Options>()
        {
            new Options("Элементы на активном виде", new FilteredElementCollector(_doc, _doc.ActiveView.Id)),
            new Options("Все элементы в проекте", new FilteredElementCollector(_doc))
        };
        Option = Options.FirstOrDefault();
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
        _model.SetOpt(Option);
        _model.Execute(Parameter,Suffix,Prefix, Indent);
        Close?.Invoke();
    }
}