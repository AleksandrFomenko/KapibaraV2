using System.Windows;
using KapibaraCore.Parameters;
using SolidIntersection.Models;
using Visibility = System.Windows.Visibility;


namespace SolidIntersection.ViewModels;

public partial class SolidIntersectionViewModel : ObservableObject
{
    private readonly Document _doc;
    private readonly SolidIntersectionModel _model;
    internal static Action Close;

    [ObservableProperty]
    private string _value;

    [ObservableProperty]
    private List<SelectedItems> _itemsList= new ();
    
    [ObservableProperty]
    private SelectedItems _selectedItem;

    [ObservableProperty]
    private List<string> _parameters;

    [ObservableProperty]
    private string _parameter;
    
    [ObservableProperty]
    private string _filterByName;
    
    [ObservableProperty]
    private bool _allItems;
    
    [ObservableProperty]
    private bool _toggleButton;
    
    [ObservableProperty]
    private Visibility _textBoxVisibility = Visibility.Visible;
    
    [ObservableProperty]
    private Visibility _toggleButtonVisibility = Visibility.Hidden;

    public SolidIntersectionViewModel()
    {
        _doc = Context.ActiveDocument;
        _model = new SolidIntersectionModel(_doc);
        
        LoadedParameters();
        LoadedFamilies();
    }
    partial void OnValueChanged(string value)
    {
        ExecuteCommand.NotifyCanExecuteChanged();
    }
    partial void OnParameterChanged(string value)
    {
        CheckParameter();
        ExecuteCommand.NotifyCanExecuteChanged();
    }
    partial void OnAllItemsChanged(bool value)
    {
        foreach (var item in ItemsList)
        {
            item.SetCheck(value);
        }
    }
    partial void OnToggleButtonChanged(bool value)
    {
        Value = value ? "1" : "0";
    }
    partial void OnSelectedItemChanged(SelectedItems selectedItems)
    {
        selectedItems.IsChecked = !selectedItems.IsChecked;
    }

    partial void OnFilterByNameChanged(string value)
    {
        ItemsList = _model.LoadedFamilies(value);
    }
    private void LoadedParameters()
    {
        Parameters = _doc.GetProjectParameters();
    }
    private void LoadedFamilies()
    {
        ItemsList = _model.LoadedFamilies(string.Empty);
    }
    private void CheckParameter()
    {
        var definition = _doc.GetProjectParameterDefinition(Parameter);

        if (definition.GetDataType().Equals(SpecTypeId.Boolean.YesNo))
        {
            TextBoxVisibility = Visibility.Hidden;
            ToggleButtonVisibility = Visibility.Visible;
        }
        else
        {
            TextBoxVisibility = Visibility.Visible;
            ToggleButtonVisibility = Visibility.Hidden;
        }
    }
    private bool CanExecuteCommand()
    {
        return Parameter != null;
    }
    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    private void Execute(Window window)
    {
        var selectedItems = ItemsList.Where(item => item.IsChecked);
        _model.Execute(selectedItems, Parameter, Value);
        Close();
    }
}
