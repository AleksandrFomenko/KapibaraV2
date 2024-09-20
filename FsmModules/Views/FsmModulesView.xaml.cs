using FsmModules.ViewModels;

namespace FsmModules.Views;

public sealed partial class FsmModulesView
{
    public FsmModulesView(FsmModulesViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}