using RiserMate.Abstractions;
using RiserMate.Entities;

namespace RiserMate.ViewModels;

public partial class RizerCreatorViewModel : ObservableObject
{
    private IModelRiserCreator _model;
    
    [ObservableProperty] 
    private List<Choice> _choices = null!;
    
    [ObservableProperty] 
    private Choice _selectedChoice;

    [ObservableProperty] 
    private List<string> _userParameters = null!;
    
    [ObservableProperty] 
    private string _selectedUserParameter = null!;
    
    [ObservableProperty]
    private List<HeatingRiser> _heatingRisers = [];

    [ObservableProperty] 
    private bool _checkAllRisers;

    
    public RizerCreatorViewModel(IModelRiserCreator model)
    { 
        _model = model;
        Choices = Enum.GetValues(typeof(Choice)).Cast<Choice>().ToList();

        UserParameters = _model.GetUserParameters();
        HeatingRisers  = _model.GetHeatingRisers();
        HeatingRisers.ForEach(r => r.ClickSelect += SelectHeatingRiser);
        HeatingRisers.ForEach(r => r.ClickShow3D += ShowHeatingRiser);
    }
    private void SelectHeatingRiser(object sender, HeatingRiser e) => _model.SelectHeatingRiser(e);
    private void ShowHeatingRiser(object sender, HeatingRiser e) => _model.Show3D(e);
    partial void OnCheckAllRisersChanged(bool value) => HeatingRisers.ForEach(r => r.IsChecked = value);

}