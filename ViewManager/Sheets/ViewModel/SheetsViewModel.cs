using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ViewManager.Sheets.Tabs;
using ViewManager.Sheets.Tabs.CreateSheets.VM;
using ViewManager.ViewModels;

namespace ViewManager.Sheets.ViewModel;

internal class SheetsViewModel: INotifyPropertyChanged
{
    private Document _doc;
    public string Header => "Менеджер листов";

    
    public ObservableCollection<ISheetsTab> Tabs { get; }
    private ISheetsTab _currentTab;
    public ISheetsTab CurrentTab
    {
        get => _currentTab;
        set
        {
            if (_currentTab != value)
            {
                _currentTab = value;
                OnPropertyChanged();
            }
        }
    }
    public SheetsViewModel(Document doc)
    {
        _doc = doc;
        
        
        Tabs = new ObservableCollection<ISheetsTab>
        {
            new CreateSheetsVM( _doc),
        };
        CurrentTab = Tabs.FirstOrDefault();
        
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}