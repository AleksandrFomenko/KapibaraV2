using LevelByFloor.ViewModels;

namespace LevelByFloor.Views;

public sealed partial class LevelByFloorView
{
    public LevelByFloorView(LevelByFloorViewModel viewModel)
    {
        DataContext = viewModel;
        LevelByFloorViewModel.Close = Close;
        InitializeComponent();
    }
}