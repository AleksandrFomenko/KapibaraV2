using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using ImportExcelByParameter.Configuration;
using ImportExcelByParameter.Models;
using Microsoft.Win32;

namespace ImportExcelByParameter.ViewModels;

public sealed class ImportExcelByParameterViewModel : INotifyPropertyChanged
{
    private Document _doc;
    private readonly ExcelByParameterModel _model;
    
    internal Config Cfg { get; set; }
    public RelayCommand StartCommand { get; }
    public RelayCommand SelectPathCommand { get; }
    internal static Action CloseWindow { get; set; }
    internal ImportExcelByParameterViewModel(Document doc, Config qwe)
    {
        var path = qwe.GetPath();
        Cfg = KapibaraCore.Configuration.Configuration.LoadConfig<Config>(path);
        
        _doc = doc;
        _model = new ExcelByParameterModel(doc);
        _categories = _model.Data.LoadCategory();
        
        
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        SelectPathCommand = new RelayCommand(
            execute: _ => SelectPath(),
            canExecute: _ => true
        );
        
        if (!File.Exists(Cfg.PathStr))
        {
            PathExcel = "File not found";
        }

        if (!string.IsNullOrEmpty(Cfg.PathStr))
        {
            try
            {
                LoadSheets(Cfg.PathStr);
            }
            catch (Exception)
            {
                Sheets = new List<string>();
            }
        }
        
        if (!string.IsNullOrEmpty(Cfg.Category))
        {
            try
            {
                LoadParameters();
            }
            catch (Exception)
            {
                Parameters = new List<string>();
            }
        }
    }
    
    public string PathExcel
    {
        get => Cfg.PathStr;
        set
        {
            Cfg.PathStr = value;
            OnPropertyChanged();
            StartCommand.RaiseCanExecuteChanged();
        }
    }

    public string SelectedCategory
    {
        get => Cfg.Category;
        set
        {
            Cfg.Category = value;
            Cfg.SaveConfig();
            OnPropertyChanged();
            StartCommand.RaiseCanExecuteChanged();
            LoadParameters();
        }
    }
    private List<string> _categories;
    public List<string> Categories
    {
        get => _categories;
        set {
            if (_categories != null && _categories != value) _categories = value;
            OnPropertyChanged();
            LoadParameters();
        }
    }

    public string Parameter
    {
        get => Cfg.Parameter;
        set
        {
            Cfg.Parameter = value;
            Cfg.SaveConfig();
            OnPropertyChanged();
            StartCommand.RaiseCanExecuteChanged();
        } 
    }
    private List<string> _parameters;
    public List<string> Parameters
    {
        get => _parameters;
        set { 
            if (_parameters == value) return;
            _parameters = value;
            OnPropertyChanged();
        }
    }
    private void LoadParameters()
    { 
        Parameters = _model.Data.LoadParameters(Cfg.Category);
    }
    private List<string> _sheets;
    public List<string> Sheets
    {
        get => _sheets;
        set { 
            if (_sheets == value) return;
            _sheets = value;
            OnPropertyChanged();
        }
    }

    public string Sheet
    {
        get => Cfg.ListStr;
        set
        {
            Cfg.ListStr = value;
            Cfg.SaveConfig();
            OnPropertyChanged();
        } 
    }

    private int _rowNumber = 1;
    public int RowNumber
    {
        get => Cfg.Number;
        set
        {
            if (_rowNumber == value) return;
            Cfg.Number = value;
            Cfg.SaveConfig();
            _rowNumber = value;
            OnPropertyChanged();
        }
    }
    private void LoadSheets(string path)
    {
        Sheets = _model.Excel.GetWorksheetNames(path);
        OnPropertyChanged(nameof(Sheets));
    }
    private bool CanExecute()
    {
        return SelectedCategory != null && Parameter != null && PathExcel != null;
    }
    private void SelectPath()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            Cfg.PathStr = openFileDialog.FileName;
            Cfg.SaveConfig();
            LoadSheets(Cfg.PathStr);
            OnPropertyChanged(nameof(PathExcel));
            StartCommand.RaiseCanExecuteChanged();
        }
    }

    private void Execute()
    {
        _model.SetParameterName(Parameter);
        _model.SetSheetName(Sheet);
        _model.SetRowNumber(RowNumber);
        _model.Execute(PathExcel,  SelectedCategory);
        CloseWindow.Invoke();
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}