using EngineeringSystems.ViewModels;

namespace EngineeringSystems.Views;

public sealed partial class EngineeringSystemsView
{
    public EngineeringSystemsView(EngineeringSystemsViewModel viewModel)
    {
        EngineeringSystemsViewModel.Close = Close;
        DataContext = viewModel;
        InitializeComponent();
    }
}