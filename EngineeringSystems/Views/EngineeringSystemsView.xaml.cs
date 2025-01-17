using EngineeringSystems.ViewModels;

namespace EngineeringSystems.Views;

public sealed partial class EngineeringSystemsView
{
    public EngineeringSystemsView(EngineeringSystemsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}