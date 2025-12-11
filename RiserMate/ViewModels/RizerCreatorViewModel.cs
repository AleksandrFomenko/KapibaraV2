using System.Collections.ObjectModel;
using System.Windows.Threading;
using KapibaraUI.ProgressBar;
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
    
    [ObservableProperty] private bool _visibleMark;
    
    // Марки
    [ObservableProperty] private List<string> _marksHeatDevice;
    [ObservableProperty] private string _selectedMarkHeatDevice;
    [ObservableProperty] private List<string> _marksPipe;
    [ObservableProperty] private string _selectedMarkPipe;
    [ObservableProperty] private List<string> _marksPipeAccessory;
    [ObservableProperty] private string _selectedMarkPipeAccessory;
    
    public RizerCreatorViewModel(IModelRiserCreator model, IConfigRiserMate config)
    { 
        _model = model;
        _config = config;
        Choices = Enum.GetValues(typeof(Choice)).Cast<Choice>().ToList();

        UserParameters        = _model.GetUserParameters();
        SelectedUserParameter = UserParameters.Contains(config.GetSelectedUserParameter()) ? config.GetSelectedUserParameter() : string.Empty;
       
        ViewTypes3D           = _model.GetTypes3D();
        
        MarksHeatDevice       = _model.GetMarksHeatDevice();
        MarksPipe             = _model.GetMarksPipe();
        MarksPipeAccessory    = _model.GetMarksPipeAccessory();
    }

    partial void OnSelectedUserParameterChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        HeatingRisers = HeatingRisersUi;
        HeatingRisersUi = _model.GetHeatingRisers(SelectedUserParameter)
            .OrderBy(x => x.Name) 
            .ToList();
        HeatingRisers = HeatingRisersUi;
        _config.SetSelectedUserParameter(value);
    }

    partial void OnFilterByNameChanged(string? value) 
        => HeatingRisers = HeatingRisersUi.Where(h => h.Name.Contains(value ?? string.Empty)).ToList();
    private void SelectHeatingRiser(object sender, HeatingRiser e) => _model.SelectHeatingRiser(e, SelectedUserParameter);
    private void ShowHeatingRiser(object sender, HeatingRiser e) => _model.Show3D(e);
    partial void OnCheckAllRisersChanged(bool value) => HeatingRisers.ForEach(r => r.IsChecked = value);
    partial void OnCreateViewChanged(bool value) => SetVisibleMark();
    partial void OnIsMarkingChanged(bool value) => SetVisibleMark();
    private void SetVisibleMark() => VisibleMark = CreateView && IsMarking;
    
    private void GetRisers()
    {
        HeatingRisersUi  = _model.GetHeatingRisers(SelectedUserParameter);
        HeatingRisersUi = _model.GetHeatingRisers(SelectedUserParameter)
            .OrderBy(x => x.Name) 
            .ToList();
        HeatingRisers    = HeatingRisersUi;
        
        HeatingRisers.ForEach(r => r.ClickSelect += SelectHeatingRiser);
        HeatingRisers.ForEach(r => r.ClickShow3D += ShowHeatingRiser);
    }

    [RelayCommand]
    private void Update() => GetRisers();
    
    
    
    
    [RelayCommand]
    private async Task Execute()
    {
        var risersToProcess = SelectedChoice == Choice.All 
            ? HeatingRisers.ToList() 
            : HeatingRisers.Where(p => p.IsChecked).ToList();

        if (risersToProcess.Count == 0) return;
        
        var progressHandle = await ProgressBar.ShowAsync(risersToProcess.Count);
        
        try
        {
            await _model.ExecuteAsync(risersToProcess, SelectedUserParameter, progressHandle);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await progressHandle.CloseAsync();
        }
        
        if (CreateView)
        {
            var progressViews = await ProgressBar.ShowAsync(risersToProcess.Count, "Создание 3D видов...");

            try
            {
                await _model.CreateViewsAsync(
                    risersToProcess,
                    SelectedUserParameter,
                    SelectedViewType3D,
                    IsMarking,
                    SelectedMarkHeatDevice,
                    SelectedMarkPipe,
                    SelectedMarkPipeAccessory,
                    progressViews);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                await progressViews.CloseAsync();
            }
        }
        
    }
    
    
    /* Без прогресс бара
    [RelayCommand]
    private void Execute()
    {
        if (SelectedChoice == Choice.All)
        {
            _model.Execute(HeatingRisers, SelectedUserParameter);

            if (!CreateView) return;

            _model.СreateViews(
                HeatingRisers,
                SelectedUserParameter,
                SelectedViewType3D,
                IsMarking,
                SelectedMarkHeatDevice,
                SelectedMarkPipe,
                SelectedMarkPipeAccessory);

            return;
        }

        _model.Execute(HeatingRisers.Where(p=> p.IsChecked).ToList(), SelectedUserParameter);

        if (!CreateView) return;

        _model.СreateViews(
            HeatingRisers.Where(p=> p.IsChecked).ToList(),
            SelectedUserParameter,
            SelectedViewType3D,
            IsMarking,
            SelectedMarkHeatDevice,
            SelectedMarkPipe,
            SelectedMarkPipeAccessory);

    }
    */
}