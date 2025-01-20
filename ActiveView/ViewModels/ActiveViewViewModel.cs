using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ActiveView.Models;
using System.Windows;
using KapibaraCore.Parameters;
using RelayCommand = KapibaraCore.RelayCommand.RelayCommand;
using Visibility = System.Windows.Visibility;

namespace ActiveView.ViewModels;

public sealed class ActiveViewViewModel : INotifyPropertyChanged
{
    private readonly Document _doc;
    private List<string> _parameters;
    private string _parameter;
    private string _value;
    private readonly ActiveViewModel _model;
    public static Action Close;
    private Visibility _textBoxVisibility = Visibility.Visible;
    private Visibility _toggleButtonVisibility = Visibility.Hidden;

    public RelayCommand StartCommand { get; }

    internal ActiveViewViewModel(Document doc)
    {
        _doc = doc ?? throw new ArgumentNullException(nameof(doc));
        _model = new ActiveViewModel(doc);
        _parameters = _model.GetParameters();
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
    }
    public List<string> Parameters
    {
        get => _parameters ??=new List<string>();
        set => SetProperty(ref _parameters, value);
    }
    public string Parameter
    {
        get => _parameter ??=string.Empty;
        set
        {
            SetProperty(ref _parameter, value);
            CheckParameter();
        }
    }
    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            SetProperty(ref _isActive, value);
            Value = value ? "1" : "0";
        }
    }
    public Visibility TextBoxVisibility
    {
        get => _textBoxVisibility;
        set
        {
            _textBoxVisibility = value;
            OnPropertyChanged();
        }
    }
    public Visibility ToggleButtonVisibility
    {
        get => _toggleButtonVisibility;
        set
        {
            _toggleButtonVisibility = value;
            OnPropertyChanged();
        }
    }
    private void CheckParameter()
    {
        var definition = _doc.GetProjectParameterDefinition(Parameter);

        if (definition.GetDataType().Equals(SpecTypeId.Boolean.YesNo))
        {
            TextBoxVisibility = Visibility.Hidden;
            ToggleButtonVisibility = Visibility.Visible;
        }
        else
        {
            TextBoxVisibility = Visibility.Visible;
            ToggleButtonVisibility = Visibility.Hidden;
        }
        OnPropertyChanged(nameof(TextBoxVisibility));
        OnPropertyChanged(nameof(ToggleButtonVisibility));
    }
    private bool CanExecute()
    {
        return true;
    }
    private void Execute()
    {
        _model.Execute(Parameter, Value);
        Close();
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }
}