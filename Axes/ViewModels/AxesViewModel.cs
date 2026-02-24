using System.Collections.ObjectModel;
using Axes.Models;

namespace Axes.ViewModels;

public sealed partial class AxesViewModel : ObservableObject
{
    private readonly IAxesModel _model;
    
    [ObservableProperty] private List<Choice?> _choices;
    [ObservableProperty] private Choice? _selectedChoice;
    
    [ObservableProperty] private ObservableCollection<Option> _options;
    [ObservableProperty] private Option? _selectedOption;

    [ObservableProperty] private bool _top;
    [ObservableProperty] private bool _left;
    [ObservableProperty] private bool _right;
    [ObservableProperty] private bool _bottom;

    private bool IsAll;
    
    public AxesViewModel(IAxesModel model)
    {
        _model         = model;
        Choices        = Choice.GetChoices();
        SelectedChoice = Choices.FirstOrDefault();
        
        Options        = new ObservableCollection<Option>(
            Enum.GetValues(typeof(Option)).Cast<Option>());
        SelectedOption = Options[0];
    }

    partial void OnSelectedOptionChanged(Option? value) => IsAll = value == Option.All;
    

    partial void OnTopChanged(bool value) => _model.HideTopOrBottom(IsAll, true, value);
    partial void OnBottomChanged(bool value) => _model.HideTopOrBottom(IsAll, false, value);
    partial void OnLeftChanged(bool value) => _model.HideRightOrLeft(IsAll, true, value);
    partial void OnRightChanged(bool value) => _model.HideRightOrLeft(IsAll, false, value);

    [RelayCommand]
    private void To2D() => _model.ChangeDatumExtent(IsAll, false);
    
    [RelayCommand]
    private void To3D() => _model.ChangeDatumExtent(IsAll, true);
}