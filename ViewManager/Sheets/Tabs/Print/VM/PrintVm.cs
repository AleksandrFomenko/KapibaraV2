using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using ViewManager.Sheets.Tabs.Print.Model;
using ViewManager.ViewModels;
using RelayCommand = KapibaraCore.RelayCommand.RelayCommand;
namespace ViewManager.Sheets.Tabs.Print.VM;

internal sealed class PrintVm:ISheetsTab, INotifyPropertyChanged
{
    private Document _doc;
    private Data _data;
    private bool _checkCombine;
    private PrintModel _model;
    private string _combineFileName;
    
    public string Header => "Печать листов";
    public ObservableCollection<FolderItem> TreeItems { get; set; }
    public RelayCommand StartCommand { get; }
    public RelayCommand SelectPathCommand { get; }
    public string PathFolder { get; set;}
    public string CombineFileNameText => "Наименование файла:";
    public string CombineFileText =>"Объединить файлы:";
    public GridLength ExportPdfBorderHeight { get; set; }
    public string CombineFileName
    {
        get => _combineFileName;
        set
        {
            SetField(ref _combineFileName, value);
            StartCommand.RaiseCanExecuteChanged();
        }
    }
    public bool CheckCombine
    {
        get => _checkCombine;
        set
        {
            SetField(ref _checkCombine, value);
            ExportPdfBorderHeight = value ? new GridLength(240, GridUnitType.Pixel) : new GridLength(170, GridUnitType.Pixel);
            OnPropertyChanged(nameof(ExportPdfBorderHeight));
            StartCommand.RaiseCanExecuteChanged();
        }
    }
    public PrintVm(Document doc, PrintModel model)
    {
        StartCommand = new RelayCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
        SelectPathCommand = new RelayCommand(
            execute: _ => SelectPath(),
            canExecute: _ => true
        );
        CheckCombine = false;
        _doc = doc;
        _model = model;
        _data = new Data(doc);
        TreeItems = _data.GetSheetOrganization();
    }
    private void SelectPath()
    {
        var dialog = new VistaFolderBrowserDialog
        {
            Description = "Выберите папку для сохранения",
            UseDescriptionForTitle = true
        };

        var result = dialog.ShowDialog();
        if (result != true) return;
        PathFolder = dialog.SelectedPath;
        OnPropertyChanged(nameof(PathFolder));
        StartCommand.RaiseCanExecuteChanged();
    }
    private bool CanExecute()
    {
        var flag1 = true;
        if (CheckCombine)
        {
            flag1 = !CombineFileName.IsNullOrEmpty();
        }
        
        return (!PathFolder.IsNullOrEmpty() && flag1);
    }
    private void Execute()
    {
        var l = _data.GetCheckedSheets(TreeItems).Select(l => (l.ElemId)).ToList();
        _model.Execute(PathFolder, l, CheckCombine, CombineFileName);
        ViewManagerViewModel.CloseWindow();
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