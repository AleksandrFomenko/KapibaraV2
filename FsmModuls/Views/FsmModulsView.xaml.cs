using FsmModuls.ViewModels;

namespace FsmModuls.Views;

public sealed partial class FsmModulsView
{
    public FsmModulsView(FsmModulsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}