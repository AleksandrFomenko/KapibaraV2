using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ViewManager.Legends.Model;
using ViewManager.Models.Entites;
using ViewManager.ViewModels;

namespace ViewManager.Legends.ViewModel;

public sealed class LegendsViewModel : INotifyPropertyChanged
{
    private Document _doc;
    public string Header => "Размещение легенд на видах";
    public string Settings => "Расположение на листе";

    public RelCommand StartCommand { get; }
    public ObservableCollection<Autodesk.Revit.DB.View> Legends { get; set; }
    
    public Autodesk.Revit.DB.View _selectedLegend;


    public Autodesk.Revit.DB.View SelectedLegend
    {
        get => _selectedLegend;
        set
        {
            if (_selectedLegend != value)
            {
                _selectedLegend = value;
                OnPropertyChanged();
                StartCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public ObservableCollection<ViewSheetItem> ViewSheets { get; } = new ObservableCollection<ViewSheetItem>();
    public ObservableCollection<string> CornersSheet { get; } = new ObservableCollection<string>()
    {
        "Правый нижний",
        "Правый верхний",
        "Левый нижний",
        "Левый верхний"
    };
    

    private string _corner;
    public string Corner
    {
        get => _corner;
        set
        {
            if (_corner != value)
            {
                _corner = value;
                OnPropertyChanged();
            }
        }
    }

    private string _filterLegends = "";

    public string FilterLegends
    {
        get => _filterLegends;
        set
        {
            if (_filterLegends != value)
            {
                _filterLegends = value;
                OnPropertyChanged();
                LoadLegend();
            }
        }
    }
    
    private string _filterViewSheet = "";

    public string FilterViewSheet
    {
        get => _filterViewSheet;
        set
        {
            if (_filterViewSheet != value)
            {
                _filterViewSheet = value;
                OnPropertyChanged();
                LoadViewSheet();
            }
        }
    }

    private double _changeX;

    public double ChangeX
    {
        get => _changeX;
        set
        {
            if (_changeX != value)
            {
                _changeX = value;
            }
        }
    }
    
    private double _changeY;

    public double ChangeY
    {
        get => _changeY;
        set
        {
            if (_changeY != value)
            {
                _changeY = value;
            }
        }
    }
    
    public LegendsViewModel(Document doc)
    {
        _doc = doc;
        LoadLegend();
        LoadViewSheet();
        Corner = CornersSheet.FirstOrDefault();
        StartCommand = new RelCommand(
            execute: _ => Execute(),
            canExecute: _ => CanExecute()
        );
    }

    private bool CanExecute()
    {
        return SelectedLegend != null;
    }
    
    private void Execute()
    {
        var selectedRevitSheets = ViewSheets
            .Where(sheet => sheet.IsChecked)
            .Select(sheet => _doc.GetElement(new ElementId(sheet.ID)) as ViewSheet)
            .Where(sheet => sheet != null)
            .ToList();
        var legendsModel = new LegendsModel(_doc, SelectedLegend);
        
        using (var tr = new Transaction(_doc))
        {
            tr.Start("Set Legends");
            legendsModel.Execute(selectedRevitSheets, _corner, _changeX / 304.8, _changeY / 304.8);
            tr.Commit();
        }
        
        ViewManagerViewModel.CloseWindow?.Invoke();
    }
    private void LoadLegend()
    {
        Legends = new ObservableCollection<Autodesk.Revit.DB.View>(
            new FilteredElementCollector(_doc)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .Cast<Autodesk.Revit.DB.View>()
                .Where(v => v.ViewType == ViewType.Legend)
                .Where(v => v.Name.Contains(_filterLegends))
        );
        OnPropertyChanged(nameof(Legends));
    }
    
    private void LoadViewSheet()
    {
        var viewSheetList = new ObservableCollection<Autodesk.Revit.DB.ViewSheet>(
            new FilteredElementCollector(_doc)
                .OfClass(typeof(Autodesk.Revit.DB.ViewSheet))
                .Cast<Autodesk.Revit.DB.ViewSheet>()
                .Where(v => v.Name.Contains(_filterViewSheet))
        );
        ViewSheets.Clear();
        foreach (var sheet in viewSheetList)
        {
            ViewSheets.Add(new ViewSheetItem
            {
                ID = sheet.Id.IntegerValue,
                IsChecked = false,
                Number = sheet.SheetNumber,
                Name = sheet.Name
            });
        }
        OnPropertyChanged(nameof(ViewSheets));
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}