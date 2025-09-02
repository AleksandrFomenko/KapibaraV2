using ExporterModels.Dialogs.AddConfiguration.Model;

namespace ExporterModels.Dialogs.AddConfiguration.ViewModel;

public partial class AddConfigurationViewModel : ObservableObject
{
    public static Action? CloseWindow;

    [ObservableProperty] private string? _buttonContent;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(ExecuteCommand))]
    private string _configName;

    private AddConfigurationModel _model;

    [ObservableProperty] private string? _placeHolderText;

    [ObservableProperty] private string? _title;


    public AddConfigurationViewModel(
        AddConfigurationModel model,
        string? title,
        string? placeHolderText,
        string? buttonContent,
        Action<string> execute)
    {
        _model = model;
        Title = title;
        _placeHolderText = placeHolderText;
        _buttonContent = buttonContent;
        Run = execute;
    }

    public event Action<string> Run;

    protected virtual void OnRun()
    {
        Run?.Invoke(ConfigName);
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    private void Execute()
    {
        OnRun();
        CloseWindow?.Invoke();
    }

    private bool CanExecute()
    {
        return ConfigName != null;
    }
}