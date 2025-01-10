using System.ComponentModel;
using System.Runtime.CompilerServices;
using ImportExcelByParameter.Models;
using Microsoft.Win32;

namespace ImportExcelByParameter.ViewModels;

public sealed class ImportExcelByParameterViewModel : INotifyPropertyChanged
{
    private Document _doc;
    private readonly ExcelByParameterModel _model;
    public RelayCommand StartCommand { get; }
    public RelayCommand SelectPathCommand { get; }
    internal static Action CloseWindow { get; set; }
    internal ImportExcelByParameterViewModel(Document doc)
    {
        _doc = doc;
        _model = new ExcelByParameterModel(doc);
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        SelectPathCommand = new RelayCommand(
            execute: _ => SelectPath(),
            canExecute: _ => true
        );
        _categories = _model.Data.LoadCategory();
    }
    
    private string _pathExcel;

    public string PathExcel
    {
        get => _pathExcel;
        set
        {
            if (_pathExcel != value)
            {
                _pathExcel = value;
                OnPropertyChanged();
                StartCommand.RaiseCanExecuteChanged();
            }
        }
    }
    private string _selectedCategory;
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;
                OnPropertyChanged();
                StartCommand.RaiseCanExecuteChanged();
                LoadParameters();
            }
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
    private string _parameter;
    public string Parameter
    {
        get => _parameter;
        set
        {
            if (_parameter == value) return;
            _parameter = value;
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
        Parameters = _model.Data.LoadParameters(SelectedCategory);
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
    private string _sheet;
    public string Sheet
    {
        get => _sheet;
        set
        {
            if (_sheet == value) return;
            _sheet = value;
            OnPropertyChanged();
        } 
    }

    private int _rowNumber = 1;
    public int RowNumber
    {
        get => _rowNumber;
        set
        {
            if (_rowNumber == value) return;
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
            _pathExcel = openFileDialog.FileName;
            LoadSheets(_pathExcel);
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
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}