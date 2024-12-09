using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}