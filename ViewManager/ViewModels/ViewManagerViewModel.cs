using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ViewManager.Kinds.ViewModel;
using ViewManager.Legends.ViewModel;
using ViewManager.Sheets.ViewModel;

namespace ViewManager.ViewModels;

public class PanelItem
{
    public string Name { get; set; }
    public object ViewModel { get; set; }
}
public sealed class ViewManagerViewModel : INotifyPropertyChanged
{
    private Document doc = Context.ActiveDocument;
    public ObservableCollection<PanelItem> Panels { get; }
    
    private PanelItem _selectedPanel;
    public static Action CloseWindow { get; set; }
    
    public PanelItem SelectedPanel
    {
        get => _selectedPanel;
        set
        {
            if (_selectedPanel != value)
            {
                _selectedPanel = value;
                OnPropertyChanged();
                CurrentViewModel = _selectedPanel?.ViewModel;
            }
        }
    }

    private object _currentViewModel;
    public object CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (_currentViewModel != value)
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }
    }

    public ViewManagerViewModel()
    {
        Panels = new ObservableCollection<PanelItem>
        {
            new PanelItem { Name = "Легенды", ViewModel = new LegendsViewModel(doc)},
            new PanelItem { Name = "Виды", ViewModel = new KindsViewModel()},
            new PanelItem { Name = "Листы", ViewModel = new SheetsViewModel(doc)}
        };
        
        SelectedPanel = Panels.FirstOrDefault();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}