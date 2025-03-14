using System.ComponentModel;
using System.Windows;
using Autodesk.Revit.Attributes;
using WorkSetLinkFiles.Commands;
using WorkSetLinkFiles.Models;

namespace WorkSetLinkFiles.ViewModels;

public sealed partial class WorkSetLinkFilesViewModel : ObservableObject
{
    private readonly Data _data;
    private readonly Document _doc;
    private readonly WorkSetLinkFilesModel _model;
    public static Action Close;

    [ObservableProperty] private bool _isCheckedAllLinks = true;
    
    [ObservableProperty]
    private string _nameText;
    
    [ObservableProperty]
    private string _prefixText;
    
    [ObservableProperty]
    private string _suffixText;
    
    [ObservableProperty]
    private string _suffix;
    
    [ObservableProperty]
    private string _prefix;
    
    [ObservableProperty]
    private string _axesText;
    
    [ObservableProperty]
    private bool _axes;
    
    [ObservableProperty]
    private string _levelText;
    
    [ObservableProperty]
    private bool _level;
    
    [ObservableProperty]
    private List<LinkFiles> _linksRevitModels;
    
    [ObservableProperty]
    private LinkFiles _linkRevitModel;

    [ObservableProperty]
    private List<string> _worksets;
    
    [ObservableProperty]
    private string _worksetLevel;
    
    [ObservableProperty]
    private string _worksetAxes;

    partial void OnIsCheckedAllLinksChanged(bool value)
    {
        LinksRevitModels.ForEach(link => link.IsChecked = value);
        StartCommand.NotifyCanExecuteChanged();
    }


    partial void OnAxesChanged(bool value) =>  StartCommand.NotifyCanExecuteChanged();
    partial void OnLevelChanged(bool value) =>  StartCommand.NotifyCanExecuteChanged();
    
    
    partial void OnLinkRevitModelChanged(LinkFiles value)
    {
        value.IsChecked = !value.IsChecked;
        StartCommand.NotifyCanExecuteChanged();
    }
    
    partial void OnPrefixChanged(string value) =>
        LinksRevitModels.ForEach(link => link.Prefix = value);

    partial void OnSuffixChanged(string value) =>
        LinksRevitModels.ForEach(link => link.Suffix = value);
    
    private List<LinkFiles> GetTrueLink()
    {
        return LinksRevitModels.Where(l => l.IsChecked).ToList();
    }

    internal WorkSetLinkFilesViewModel(Document doc)
    {
        _doc = doc;
        _data = new Data(doc);
        _model = new WorkSetLinkFilesModel(doc);
        
        NameText = "Наименование";
        PrefixText = "Префикс";
        SuffixText = "Суффикс";
        AxesText = "Перенести оси в рабочий набор:";
        LevelText = "Перенести уровни в рабочий набор:";


        LinksRevitModels = _data.GetLinks();
        Worksets = _data.GetWorksets();
        WorksetLevel = Worksets.FirstOrDefault() ?? string.Empty;
        WorksetAxes = Worksets.FirstOrDefault() ?? string.Empty;
        foreach (var link in LinksRevitModels)
        {
            link.PropertyChanged += Link_PropertyChanged;
            link.IsCheckedChanged += () => StartCommand.NotifyCanExecuteChanged();
        }
    }
    
    private bool CanStartCommand()
    {
        return Axes || Level || LinksRevitModels.Any(link => link.IsChecked);
    }
    [RelayCommand(CanExecute = nameof(CanStartCommand))]
    private void Start(Window window)
    {
        using (var t = new Transaction(_doc,"Set workset"))
        {
            t.Start();
            try
            {
                _model.SetLinksWorkset(GetTrueLink(), Suffix, Prefix);
            }
            catch
            {
                // ignored
            }
            try
            {
                _model.SetWorkset(
                    Level ? WorksetLevel : string.Empty, 
                    BuiltInCategory.OST_Levels
                );
            }
            catch
            {
                // ignored
            }
            try
            {
                _model.SetWorkset(
                    Axes ? WorksetAxes : string.Empty, 
                    BuiltInCategory.OST_Grids
                );
            }
            catch
            {
                // ignored
            }
            t.Commit();
        }
        Close?.Invoke();
    }
    private void Link_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LinkFiles.IsChecked))
        {
            bool anyChecked = LinksRevitModels.Any(link => link.IsChecked);
            if (IsCheckedAllLinks && IsCheckedAllLinks != anyChecked)
            {
                IsCheckedAllLinks = anyChecked;
            }
            StartCommand.NotifyCanExecuteChanged();
        }
    }
}