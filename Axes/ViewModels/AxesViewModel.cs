namespace Axes.ViewModels;

public sealed partial class AxesViewModel : ObservableObject
{
    [ObservableProperty] private List<Choice> _choices;
    
    [ObservableProperty]
    private Choice _selectedChoice;
    
    
    public AxesViewModel()
    {
        Choices = Choice.GetChoices();
        SelectedChoice = Choices.FirstOrDefault()!;
    }
    
}