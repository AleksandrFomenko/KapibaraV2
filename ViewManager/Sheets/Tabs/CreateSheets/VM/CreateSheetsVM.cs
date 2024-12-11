using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ViewManager.ViewModels;

namespace ViewManager.Sheets.Tabs.CreateSheets.VM;

internal class CreateSheetsVM: ISheetsTab, INotifyPropertyChanged
{
    private Document _doc;
    public string Header => "Создание листов";
    private RelCommand StartCommand { get; }
    private List<SheetsType> _titleBlocks;

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

    private bool _isSystemParameter = true;
    public bool IsSystemParameter
    {
        get => _isSystemParameter;
        set
        {
            if (_isSystemParameter != value)
            {
                _isSystemParameter = value;
                OnPropertyChanged(nameof(IsSystemParameter));
                UpdateRowHeights();
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
                OnPropertyChanged(nameof(UserParameterIsVisible));
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
                OnPropertyChanged(nameof(MyRowHeight));
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
                OnPropertyChanged(nameof(MyRow2Height));
            }
        }
    }

    public CreateSheetsVM(Document doc)
    {
        _doc = doc;
        StartCommand = new RelCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        LoadTitleBlocks();
    }

    
    private bool CanExecute()
    {
        return true;
    }
    
    private void Execute()
    {
        ViewManagerViewModel.CloseWindow?.Invoke();
    }

    private void LoadTitleBlocks()
    {
        var x = new FilteredElementCollector(_doc)
            .OfCategory(BuiltInCategory.OST_TitleBlocks)
            .WhereElementIsElementType()
            .ToElements();
        List<SheetsType> sheetsTypes = new List<SheetsType>();
        if (x.Any())
        {
            foreach (var element in x)
            {
                var familyNameParameter = element.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM);
                if (familyNameParameter == null || string.IsNullOrEmpty(familyNameParameter.AsString()))
                {
                    continue;
                }
                var typeNameParameter = element.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_NAME);
                if (typeNameParameter == null || string.IsNullOrEmpty(typeNameParameter.AsString()))
                {
                    continue;
                }

                var result = $"{familyNameParameter.AsString()} : {typeNameParameter.AsString()}";
                sheetsTypes.Add(new SheetsType() { Id = element.Id.IntegerValue, Name = result });
            }
        }
        TitleBlocks = sheetsTypes; 
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
        if (IsSystemParameter)
        {
            UserParameterIsVisible = true;
        }
        else
        {
            UserParameterIsVisible = false;
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}