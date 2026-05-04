using System.IO;
using ImportExcelByParameter.Configuration;
using ImportExcelByParameter.Enums;
using ImportExcelByParameter.Models;
using Microsoft.Win32;

namespace ImportExcelByParameter.ViewModels;

public sealed partial class ImportExcelByParameterViewModel : ObservableObject
{
    private readonly ExcelByParameterModel _model;
    private Config Cfg { get; set; }
    private readonly Document _doc;

    internal static Action CloseWindow { get; set; }

    [ObservableProperty] private List<string> _categories;
    [ObservableProperty] private List<string> _parameters;
    [ObservableProperty] private List<string> _sheets;
    [ObservableProperty] private string _parameterFilter = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private string _pathExcel;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private string _selectedCategory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private string _parameter;
    
    [ObservableProperty] private SelectionMode _selectionMode = SelectionMode.AllOnActiveView;
    public IEnumerable<SelectionMode> SelectionModes =>
        Enum.GetValues(typeof(SelectionMode)).Cast<SelectionMode>();

    [ObservableProperty] private string _sheet;
    [ObservableProperty] private int _rowNumber;
    
    public bool IsCategoryVisible => SelectionMode == SelectionMode.ByCategory;

    public ImportExcelByParameterViewModel(Document doc, Config config)
    {
        _doc = doc;
        Cfg = KapibaraCore.Configuration.Configuration.LoadConfig<Config>(config.GetPath());
        _model = new ExcelByParameterModel(doc);
        _categories = _model.Data.LoadCategory();

        if (Cfg == null) return;

        _pathExcel = File.Exists(Cfg.PathStr) ? Cfg.PathStr : "File not found";
        _selectedCategory = Cfg.Category;
        _parameter = Cfg.Parameter;
        _sheet = Cfg.ListStr;
        _rowNumber = Cfg.Number;

        if (!string.IsNullOrEmpty(Cfg.PathStr))
        {
            try { _sheets = _model.Excel.GetWorksheetNames(Cfg.PathStr); }
            catch { _sheets = []; }
        }

        if (string.IsNullOrEmpty(Cfg.Category)) return;
        try { _parameters = _model.Data.LoadAllParameters(_doc);; }
        catch { _parameters = []; }
    }
    
    partial void OnSelectionModeChanged(SelectionMode value)
    {
        OnPropertyChanged(nameof(IsCategoryVisible));
        LoadParameters();
    }

    partial void OnPathExcelChanged(string value) =>
        Cfg.PathStr = value;

    partial void OnSelectedCategoryChanged(string value)
    {
        Cfg.Category = value;
        Cfg.SaveConfig();
        LoadParameters();
    }
    
    partial void OnParameterFilterChanged(string value) => LoadParameters();

    partial void OnParameterChanged(string value)
    {
        Cfg.Parameter = value;
        Cfg.SaveConfig();
    }

    partial void OnSheetChanged(string value)
    {
        Cfg.ListStr = value;
        Cfg.SaveConfig();
    }

    partial void OnRowNumberChanged(int value)
    {
        Cfg.Number = value;
        Cfg.SaveConfig();
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void Start()
    {
        _model.SetParameterName(Parameter);
        _model.SetSheetName(Sheet);
        _model.SetRowNumber(RowNumber);
        _model.Execute(PathExcel, SelectedCategory, SelectionMode);
        CloseWindow.Invoke();
    }

    [RelayCommand]
    private void SelectPath()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog() != true) return;

        PathExcel = dialog.FileName;
        Cfg.SaveConfig();
        
        try { Sheets = _model.Excel.GetWorksheetNames(PathExcel); }
        catch { Sheets = []; }
    }

    private bool CanExecute() =>
        !string.IsNullOrEmpty(Parameter) &&
        !string.IsNullOrEmpty(PathExcel) &&
        (SelectionMode != SelectionMode.ByCategory || !string.IsNullOrEmpty(SelectedCategory));

    private void LoadParameters()
    {
        var all = SelectionMode == SelectionMode.ByCategory
            ? _model.Data.LoadParameters(SelectedCategory)
            : _model.Data.LoadAllParameters(_doc);

        Parameters = string.IsNullOrEmpty(ParameterFilter)
            ? all
            : all.Where(p => p.Contains(ParameterFilter, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}