using System.ComponentModel;
using System.Runtime.CompilerServices;
using ActiveView.Handler;
using ActiveView.Models;
using KapibaraCore.Parameters;
using RelayCommand = KapibaraCore.RelayCommand.RelayCommand;
using Visibility = System.Windows.Visibility;

namespace ActiveView.ViewModels;

public sealed partial class ActiveViewViewModel : ObservableObject
{
    [ObservableProperty] private List<string> _parameters;
    [ObservableProperty] private string _parameter;
    [ObservableProperty] private string _value;
    [ObservableProperty] private bool _boolValue = true;
    [ObservableProperty] private string _headingParameterSelection = "Выбор параметра";
    [ObservableProperty] private string _headingOptionsSelection = "Выбор опции";
    [ObservableProperty] private List<Option> _options;
    [ObservableProperty] private Option _selectionOption;
    [ObservableProperty] private string _headingValue = "Значение";
    [ObservableProperty] private string _headingNotEmpty = "Пропустить заполненное";
    [ObservableProperty] private bool _skipNotEmpty = false;
    [ObservableProperty] private bool _isTextBoxVisible = true;
    [ObservableProperty] private bool _isToggleVisible = false;
    private Document Document { get; set; }

    private readonly IModelActiveView _model;
    
    public ActiveViewViewModel(IModelActiveView model, Document document)
    {
        Document = document;
        _model = model;
        _parameters = _model.GetParameters();
        Options =
        [
            new Option("Все на виде", true),
            new Option("Выбранные", false)
        ];
        SelectionOption = Options.FirstOrDefault();
    }

    partial void OnBoolValueChanged(bool value)
    {
        Value = value ? "1" : "0";
    }
    
    partial void OnParameterChanged(string value)
    {
        var definition = Document.GetProjectParameterDefinition(value);
        if (definition.GetDataType().Equals(SpecTypeId.Boolean.YesNo))
        {
            IsTextBoxVisible = false;
            IsToggleVisible = !IsTextBoxVisible;
            Value = IsToggleVisible ?  "1" : "0";
        }
        else
        {
            IsTextBoxVisible = true;
            IsToggleVisible = !IsTextBoxVisible;
        }
    }

    [RelayCommand]
    private void Execute()
    {
        _model.Execute(Parameter, Value, SkipNotEmpty, SelectionOption);
    }
}

public class Option (string name, bool isAll)
{
    public string Name { get; set; } = name;
    public bool IsAll { get; set; } =  isAll;
}