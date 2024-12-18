using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ViewManager.Sheets.Tabs.CreateSheets.Model;


namespace ViewManager.Sheets.Tabs.CreateSheets.VM;

internal class CreateSheetsVM: ISheetsTab, INotifyPropertyChanged
{
    private Document _doc;
    public string Header => "Создание листов";
    public RelCommand StartCommand { get; }
    private List<SheetsType> _titleBlocks;
    private CreateSheetsModel _model;
    
    public CreateSheetsVM(Document doc)
    {
        _doc = doc;
        StartCommand = new RelCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        _model = new CreateSheetsModel(_doc);
        LoadTitleBlocks();
    }

    public List<SheetsType> TitleBlocks
    {
        get => _titleBlocks;
        set
        {
            if (_titleBlocks != value)
            {
                _titleBlocks = value;
                OnPropertyChanged();
            }
        }
    }

    private SheetsType _titleBlock;
    public SheetsType TitleBlock
    {
        get => _titleBlock;
        set
        {
            if (_titleBlock != value)
            {
                _titleBlock = value;
                OnPropertyChanged();
                LoadParametersTitleBlock(_titleBlock.Id);
            }
        }
    }

    private bool _isSystemParameter = true;
    public bool IsSystemParameter
    {
        get => _isSystemParameter;
        set
        {
            if (_isSystemParameter != value)
            {
                _isSystemParameter = value;
                OnPropertyChanged();
                UpdateRowHeights();
            }
        }
    }

    private List<string> _parameters;
    public List<string> Parameters
    {
        get => _parameters;
        set
        {
            if (_parameters != value)
            {
                _parameters = value;
                OnPropertyChanged();
            }
        }
    }

    private string _parameter;

    public string Parameter
    {
        get => _parameter;
        set
        {
            if (_parameter != value)
            {
                _parameter = value;
                OnPropertyChanged();
            }
        }
    }
    
    private int _startValue;
    public int StartValue
    {
        get => _startValue;
        set
        {
            if (_startValue != value)
            {
                _startValue = value;
                OnPropertyChanged();
            }
        }
    }

    private int _count;

    public int Count
    {
        get => _count;
        set
        {
            if (_count != value)
            {
                _count = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _userParameterIsVisible = false;
    public bool UserParameterIsVisible
    {
        get => _userParameterIsVisible;
        set
        {
            if (_userParameterIsVisible != value)
            {
                _userParameterIsVisible = value;
                OnPropertyChanged();
                UpdateRowHeights();
                UpdateVisibleUserParameter();
            }
        }
    }
    
    private GridLength _myRowHeight = new GridLength(90);
    public GridLength MyRowHeight
    {
        get => _myRowHeight;
        set
        {
            if (_myRowHeight != value)
            {
                _myRowHeight = value;
                OnPropertyChanged();
            }
        }
    }

    private GridLength _myRow2Height = new GridLength(0);
    public GridLength MyRow2Height
    {
        get => _myRow2Height;
        set
        {
            if (_myRow2Height != value)
            {
                _myRow2Height = value;
                OnPropertyChanged();
            }
        }
    }
    private void UpdateRowHeights()
    {
        if (IsSystemParameter)
        {
            MyRowHeight = new GridLength(90);
            MyRow2Height = new GridLength(0);
        }
        else
        {
            MyRowHeight = new GridLength(180);
            MyRow2Height = new GridLength(1, GridUnitType.Star);
        }
    }
    private void UpdateVisibleUserParameter()
    {
        UserParameterIsVisible = IsSystemParameter;
    }

    private void LoadTitleBlocks()
    {
        TitleBlocks = _model._data.TitleBlocks();
    }
    private void LoadParametersTitleBlock(int titleBlockId)
    {
        var elemId = new ElementId(titleBlockId);
        var element = _doc.GetElement(elemId);
        Parameters = KapibaraCore.Parameters.Parameters.GetParameterFromFamily(_doc, element)
            .Select(x => x.Definition.Name).ToList();
    }
    private bool CanExecute()
    {
        return true;
    }
    
    private void Execute()
    {
        var title = new ElementId(_titleBlock.Id);
        Action q = () => _model.Make(title, _count, _startValue);
        _model.MakeSheets(q, "Create Sheets");
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}