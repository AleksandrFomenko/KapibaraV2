using ViewByParameter.ViewModels;

namespace ViewByParameter.Views;

public sealed partial class ViewByParameterView
{
    public ViewByParameterView(ViewByParameterViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}