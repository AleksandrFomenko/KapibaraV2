using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using KapibaraCore.Parameters;
using RelayCommand = KapibaraCore.RelayCommand.RelayCommand;

namespace EngineeringSystems.ViewModels;

public sealed class EngineeringSystemsViewModel : INotifyPropertyChanged
{
    private readonly Document _doc;
    private GridLength _firstColumnWidth;
    private GridLength _secondColumnWidth;
    private double _windowWidth;
    private double _windowHeight;
    private List<SystemParameters> _systemParameters;
    private SystemParameters _systemParameter;
    private List<string> _userParameters;
    private string _userParameter;
    private List<Options> _options;
    private Options _option;
    
    public RelayCommand StartCommand { get; }
    public GridLength FirstColumnWidth
    {
        get => _firstColumnWidth;
        private set => SetField(ref _firstColumnWidth, value);
    }
    public GridLength SecondColumnWidth
    {
        get => _secondColumnWidth;
        set => SetField(ref _secondColumnWidth, value);
    }
    public double WindowWidth
    {
        get =>  _windowWidth;
        set => SetField(ref  _windowWidth, value);
    }
    public double WindowHeight
    {
        get => _windowHeight;
        set => SetField(ref _windowHeight, value);
    }
    public List<SystemParameters> SystemParameters
    {
        get => _systemParameters;
        private set => SetField(ref _systemParameters, value);
    }
    public SystemParameters SystemParameter
    {
        get => _systemParameter;
        set => SetField(ref _systemParameter, value);
    }
    public List<string> UserParameters
    {
        get => _userParameters;
        set => SetField(ref _userParameters, value);
    }

    public string UserParameter
    {
        get => _userParameter;
        set => SetField(ref _userParameter, value);
    }
    public List<Options> Options
    {
        get => _options;
        private set => SetField(ref _options, value);
    }
    public Options Option
    {
        get => _option;
        set
        {
            SetField(ref _option, value);
            CheckOptions();
        }
    }
    internal EngineeringSystemsViewModel(Document doc)
    {
        _doc = doc;
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        
        FirstColumnWidth = new GridLength(1, GridUnitType.Star);
        SecondColumnWidth = new GridLength(0, GridUnitType.Pixel);

        SystemParameters = new List<SystemParameters>()
        {
            new SystemParameters("Имя системы"),
            new SystemParameters("Сокращение системы")
        };
        SystemParameter = SystemParameters[0];
        UserParameters = _doc.GetProjectParameters(SpecTypeId.String.Text).ToList();
        Options = new List<Options>()
        {
            new Options("Записывать в элементы на активном виде", 400, 450,
                new GridLength(1, GridUnitType.Star), 
                new GridLength(0, GridUnitType.Pixel)),
            new Options("Выбрать систему", 800,600,
                new GridLength(0.5, GridUnitType.Star),
                new GridLength(1, GridUnitType.Star))
        };
        Option = Options[0];
    }


    private void CheckOptions()
    {
        WindowWidth = Option.Width;
        WindowHeight = Option.Height;
        FirstColumnWidth = Option.FirstColumnWidth;
        SecondColumnWidth = Option.SecondColumnWidth;
    }
    private bool CanExecute()
    {
        return true;
    }
    private void Execute()
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}