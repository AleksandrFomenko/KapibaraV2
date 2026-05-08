using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImportExcelByParameter.Configuration;
using ImportExcelByParameter.Enums;
using ImportExcelByParameter.Models;
using Microsoft.Win32;

namespace ImportExcelByParameter.ViewModels;

public sealed partial class ImportExcelByParameterViewModel : ObservableObject
{
    private readonly ExcelByParameterModel _model;
    private Config Cfg { get; set; }

    internal static Action CloseWindow { get; set; }

    [ObservableProperty] private List<string> _categories;
    [ObservableProperty] private List<string> _parameters;
    [ObservableProperty] private List<string> _sheets;
    [ObservableProperty] private string _parameterFilter = string.Empty;
    
    
    [ObservableProperty] private int _currentProgress;
    [ObservableProperty] private int _maxProgress;
    [ObservableProperty] private bool _isIndeterminate;

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

    public ImportExcelByParameterViewModel(Config config, ExcelByParameterModel model)
    {
        Cfg = KapibaraCore.Configuration.Configuration.LoadConfig<Config>(config.GetPath());
        _model = model;

        if (Cfg != null)
        {
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
        }

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            Categories = await _model.Data.LoadCategoryAsyncEvent.RaiseAsync();
            await LoadParameters();
        }
        catch (Exception e)
        {
            // ignored
        }
    }
    
    partial void OnSelectionModeChanged(SelectionMode value)
    {
        OnPropertyChanged(nameof(IsCategoryVisible));
        _ = LoadParameters();
    }

    partial void OnPathExcelChanged(string value) =>
        Cfg.PathStr = value;

    partial void OnSelectedCategoryChanged(string value)
    {
        Cfg.Category = value;
        Cfg.SaveConfig();
        _ = LoadParameters();
    }
    
    partial void OnParameterFilterChanged(string value) => _ = LoadParameters();

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

        CurrentProgress = 0;

        var isFirst = true;
        var progress = new Progress<int>(value =>
        {
            if (isFirst) { MaxProgress = value; isFirst = false; }
            else CurrentProgress = value;
        });

         _model.Execute(PathExcel, SelectedCategory, SelectionMode, progress);
       
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

    private async Task LoadParameters()
    {
        var all = SelectionMode == SelectionMode.ByCategory
            ? await _model.Data.LoadParametersAsyncEvent.RaiseAsync(SelectedCategory)
            : await _model.Data.LoadAllParametersAsyncEvent.RaiseAsync();

        Parameters = string.IsNullOrEmpty(ParameterFilter)
            ? all
            : all.Where(p => p.Contains(ParameterFilter, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}