using System.Windows;
using Autodesk.Revit.Attributes;
using WorkSetLinkFiles.Models;

namespace WorkSetLinkFiles.ViewModels;

public sealed partial class WorkSetLinkFilesViewModel : ObservableObject
{
    private Data _data;
    private Document _doc;
    private WorkSetLinkFilesModel _model;
    public static Action Close;
    
    [ObservableProperty]
    private bool _isCheckedAllLinks;
    
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
    
    partial void OnIsCheckedAllLinksChanged(bool value) =>
        LinksRevitModels.ForEach(link => link.IsChecked = value);
    
    partial void OnLinkRevitModelChanged(LinkFiles value) => value.IsChecked = !value.IsChecked;

    partial void OnPrefixChanged(string value) =>
        LinksRevitModels.ForEach(link => link.Prefix = value);

    partial void OnSuffixChanged(string value) =>
        LinksRevitModels.ForEach(link => link.Suffix = value);

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
    }

    private List<LinkFiles> GetTrueLink()
    {
        return LinksRevitModels.Where(l => l.IsChecked).ToList();
    }

    private bool CanStartCommand()
    {
        return false;
    }
    [RelayCommand(CanExecute = nameof(CanStartCommand))]
    private void Start(Window window)
    {
        using (var t = new Transaction(_doc,"Set workset"))
        {
            t.Start();
            _model.SetLinksWorkset(GetTrueLink());
            t.Commit();
        }
        Close?.Invoke();
    }
}