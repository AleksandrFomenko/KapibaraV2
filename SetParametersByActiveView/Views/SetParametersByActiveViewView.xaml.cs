using SetParametersByActiveView.ViewModels;

namespace SetParametersByActiveView.Views;

public sealed partial class SetParametersByActiveViewView
{
    public SetParametersByActiveViewView(SetParametersByActiveViewViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}