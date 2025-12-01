using System.Collections.ObjectModel;
using RiserMate.Abstractions;
using RiserMate.Entities;

namespace RiserMate.ViewModels;

public partial class RizerCreatorViewModel : ObservableObject
{
    private readonly IModelRiserCreator _model;
    private List<HeatingRiser> HeatingRisersUi { get; set; }
    private IConfigRiserMate _config;
    
    [ObservableProperty] private List<Choice> _choices = null!;
    
    [ObservableProperty] private Choice _selectedChoice;

    [ObservableProperty] private List<string> _userParameters = null!;
    
    [ObservableProperty] private string _selectedUserParameter = null!;
    
    [ObservableProperty] private List<HeatingRiser> _heatingRisers = [];

    [ObservableProperty] private bool _checkAllRisers;
    
    [ObservableProperty] private string? _filterByName;
    
    [ObservableProperty] private bool _createView;

    [ObservableProperty] private List<string> _viewTypes3D;
    
    [ObservableProperty] private string _selectedViewType3D;
    
    [ObservableProperty] private bool _isMarking;

    [ObservableProperty] private List<string> _marksHeatDevice;
    
    
    public RizerCreatorViewModel(IModelRiserCreator model, IConfigRiserMate config)
    { 
        _model = model;
        _config = config;
        Choices = Enum.GetValues(typeof(Choice)).Cast<Choice>().ToList();

        UserParameters = _model.GetUserParameters();
        SelectedUserParameter = UserParameters.Contains(config.GetSelectedUserParameter()) ? config.GetSelectedUserParameter() : string.Empty;
        HeatingRisersUi  = _model.GetHeatingRisers(SelectedUserParameter);
        HeatingRisers = HeatingRisersUi;
        MarksHeatDevice = _model.GetMarksHeatDevice();
        
        HeatingRisers.ForEach(r => r.ClickSelect += SelectHeatingRiser);
        HeatingRisers.ForEach(r => r.ClickShow3D += ShowHeatingRiser);
    }

    partial void OnSelectedUserParameterChanged(string value)
    {
        HeatingRisersUi  = _model.GetHeatingRisers(SelectedUserParameter);
        HeatingRisers = HeatingRisersUi;
        _config.SetSelectedUserParameter(value);
    }

    partial void OnFilterByNameChanged(string? value) 
        => HeatingRisers = HeatingRisersUi.Where(h => h.Name.Contains(value ?? string.Empty)).ToList();
    private void SelectHeatingRiser(object sender, HeatingRiser e) => _model.SelectHeatingRiser(e, SelectedUserParameter);
    private void ShowHeatingRiser(object sender, HeatingRiser e) => _model.Show3D(e);
    partial void OnCheckAllRisersChanged(bool value) => HeatingRisers.ForEach(r => r.IsChecked = value);

    [RelayCommand]
    private void Execute()
    {
        if (SelectedChoice == Choice.All)
        {
            _model.Execute(HeatingRisers, SelectedUserParameter);
            return;
        }
        
        _model.Execute(HeatingRisers.Where(p=> p.IsChecked).ToList(), SelectedUserParameter);
    }
}