using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace ViewManager.Sheets.Tabs.Print.Model;

internal class Data
{
    private Document _doc;
    internal Data(Document doc)
    {
        _doc = doc;
    }
    internal List<SheetItem> GetCheckedSheets(IEnumerable<FolderItem> folders)
    {
        var result = new List<SheetItem>();
        foreach (var folder in folders)
        {
            result.AddRange(folder.Sheets.Where(s => s.IsChecked));
            result.AddRange(GetCheckedSheets(folder.SubFolders));
        }
        return result;
    }
    
    internal ObservableCollection<FolderItem> GetSheetOrganization()
    {
        var result = new ObservableCollection<FolderItem>();
        var bo = BrowserOrganization.GetCurrentBrowserOrganizationForSheets(_doc);
        var sheets = new FilteredElementCollector(_doc)
            .OfClass(typeof(ViewSheet))
            .WhereElementIsNotElementType()
            .ToElements();
        foreach (var sheet in sheets)
        {
            var folderChain = bo.GetFolderItems(sheet.Id);
            ObservableCollection<FolderItem> currentCollection = result;
            FolderItem currentFolder = null;
            foreach (var folder in folderChain)
            {
                var foundFolder = currentCollection.FirstOrDefault(f => f.Name == folder.Name);
                if (foundFolder == null)
                {
                    foundFolder = new FolderItem { Name = folder.Name };
                    currentCollection.Add(foundFolder);
                }

                currentFolder = foundFolder;
                currentCollection = foundFolder.SubFolders;
            }

            if (currentFolder != null)
            {
                if (sheet is ViewSheet viewSheet)
                {
                    var sheetItem = new SheetItem
                    {
                        Name = viewSheet.Name,
                        Number = viewSheet.SheetNumber,
                        ElemId = viewSheet.Id
                    };
                    currentFolder.Sheets.Add(sheetItem);
                }
            }
            else
            {
                var unorganizedFolder = result.FirstOrDefault(f => f.Name == "йцу");
                if (unorganizedFolder == null)
                {
                    unorganizedFolder = new FolderItem { Name = "йцу" };
                    result.Add(unorganizedFolder);
                }

                if (sheet is ViewSheet viewSheet)
                {
                    var sheetItem = new SheetItem
                    {
                        Name = viewSheet.Name,
                        Number = viewSheet.SheetNumber,
                        ElemId = viewSheet.Id
                    };
                    unorganizedFolder.Sheets.Add(sheetItem);
                }
            }
        }
        return result;
    }
}

public class FolderItem : INotifyPropertyChanged
{
    private bool _isChecked;
    public string Name { get; set; }
    public IEnumerable<object> AllChildren => SubFolders.Cast<object>().Concat(Sheets);
    
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value) return;
            _isChecked = value;
            OnPropertyChanged();
            foreach (var childFolder in SubFolders)
            {
                childFolder.IsChecked = value;
            }
            foreach (var sheet in Sheets)
            {
                sheet.IsChecked = value;
            }
        }
    }

    public ObservableCollection<FolderItem> SubFolders { get; set; } = new ObservableCollection<FolderItem>();
    public ObservableCollection<SheetItem> Sheets { get; set; } = new ObservableCollection<SheetItem>();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


public class SheetItem : INotifyPropertyChanged
{
    private bool _isChecked;
    public string Name { get; set; }
    public string Number { get; set; }
    public ElementId ElemId  { get; set; }
    
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value) return;
            _isChecked = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}