using System.Windows;
using KapibaraCore.Parameters;
using SolidIntersection.Models;
using Visibility = System.Windows.Visibility;


namespace SolidIntersection.ViewModels;

public partial class SolidIntersectionViewModel : ObservableObject
{
    private readonly Document _doc;
    private readonly ISolidIntersectionModel _model;
    internal static Action Close;

    [ObservableProperty] private string _value;
    [ObservableProperty] private List<SelectedItems> _itemsList= [];
    [ObservableProperty] private SelectedItems _selectedItem;
    [ObservableProperty] private List<Project> _projects;
    [ObservableProperty] private Project _project;
    [ObservableProperty] private List<string> _parameters;
    [ObservableProperty] private string _parameter;
    [ObservableProperty] private string _filterByName = string.Empty;
    [ObservableProperty] private bool _allItems;
    [ObservableProperty] private bool _toggleButton;
    [ObservableProperty] private bool _oneValueForEveryone;
    [ObservableProperty] private Visibility _showBorderWithValue = Visibility.Hidden;
    [ObservableProperty] private Visibility _textBoxVisibility = Visibility.Visible;
    [ObservableProperty] private Visibility _toggleButtonVisibility = Visibility.Hidden;

    public SolidIntersectionViewModel(ISolidIntersectionModel model)
    {
        _doc = Context.ActiveDocument;
        _model = model;
        Projects = _model.LoadedProject();
        Project = Projects.LastOrDefault() ?? new Project("Текущий файл", 1);
        Parameters = _model.LoadedParameters();
        ItemsList = _model.LoadedFamilies(string.Empty, Project);
    }
    partial void OnValueChanged(string value) => ExecuteCommand.NotifyCanExecuteChanged();

    partial void OnOneValueForEveryoneChanged(bool value)
    {
        ShowBorderWithValue = value ? Visibility.Visible : Visibility.Hidden;
        ItemsList.ForEach(item =>
        {
            item.VisibleTextBox = value ? Visibility.Hidden : Visibility.Visible;
        });

    }

    partial void OnProjectChanged(Project value)
    {
        ItemsList = _model.LoadedFamilies(FilterByName, Project);
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
    partial void OnSelectedItemChanged(SelectedItems value)
    {
        value.IsChecked = ! value.IsChecked;
    }

    partial void OnFilterByNameChanged(string value)
    {
        ItemsList = _model.LoadedFamilies(value, Project);
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
    private void Execute()
    {
        var selectedItems = ItemsList.Where(item => item.IsChecked);
        var flag1 = OneValueForEveryone;
        if (flag1)
        {
            _model.Execute(selectedItems, Parameter, Value, Project);
        }
        else
        {
            _model.Execute(selectedItems, Parameter, Project);
        }

        Close();
    }
}
