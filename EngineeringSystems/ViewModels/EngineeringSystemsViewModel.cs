using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using EngineeringSystems.Model;
using KapibaraCore.Parameters;
using RelayCommand = KapibaraCore.RelayCommand.RelayCommand;

namespace EngineeringSystems.ViewModels;

public sealed class EngineeringSystemsViewModel : INotifyPropertyChanged
{
    private readonly Document _doc;
    private Data _data;
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
    private List<EngineeringSystem> _engineeringSystems;
    private EngineeringSystem _engineeringSystem;
    private string _filterByName = string.Empty;
    private bool _isCheckedAllSystems;
    private bool _сreateView;
    
    
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
        set
        {
            SetField(ref _userParameter, value);
            StartCommand.RaiseCanExecuteChanged();
        }
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
    public List<EngineeringSystem> EngineeringSystems
    {
        get => _engineeringSystems;
        set => SetField(ref _engineeringSystems, value);
    }
    public EngineeringSystem EngineeringSystem
    {
        get => _engineeringSystem;
        set
        {
            value.IsChecked = !value.IsChecked;
            SetField(ref _engineeringSystem, value);
        }
    }
    public string FilterByName
    {
        get => _filterByName;
        set
        {
            SetField(ref _filterByName, value);
            ReloadEngineeringSystems();
        }
    }
    public bool IsCheckedAllSystems
    {
        get => _isCheckedAllSystems;
        set
        {
            SetField(ref _isCheckedAllSystems, value);
            foreach (var engineeringSystem in EngineeringSystems)
            {
                engineeringSystem.IsChecked = value;
            }
        }
    }
    public bool CreateView
    {
        get => _сreateView;
        set => SetField(ref _сreateView, value);
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
            new Options("Записать в элементы на активном виде", 400, 500,
                new GridLength(1, GridUnitType.Star), 
                new GridLength(0, GridUnitType.Pixel), 
                true
                ),
            new Options("Выбрать систему", 1000,600,
                new GridLength(0.5, GridUnitType.Star),
                new GridLength(1, GridUnitType.Star),
                false)
        };
        Option = Options[0];

        _data = new Data(_doc);
        ReloadEngineeringSystems();
    }
    private void ReloadEngineeringSystems()
    {
        EngineeringSystems = _data.GetSystems(FilterByName);
    }
    private void CheckOptions()
    {
        WindowWidth = Option.Width;
        WindowHeight = Option.Height;
        FirstColumnWidth = Option.FirstColumnWidth;
        SecondColumnWidth = Option.SecondColumnWidth;
    }
    private List<string> GetCheckedSystemNames(bool flag)
    {
        if (flag)
        {
            return EngineeringSystems
                .Where(system => system.IsChecked)
                .Select(system => system.NameSystem)
                .ToList();
        }
        
        return EngineeringSystems
            .Where(system => system.IsChecked)
            .Select(system => system.CutSystemName)
            .ToList();
    }
    private bool CanExecute()
    {
        return UserParameter != null;
    }
    private void Execute()
    {
        var model = new EngineeringSystemsModel(_doc, Option);
        var flag = SystemParameter.Name == "Имя системы" ? true : false;
        var systemString = GetCheckedSystemNames(flag);
        model.Execute(systemString, UserParameter, flag, CreateView);
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