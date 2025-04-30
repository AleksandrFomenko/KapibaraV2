using Insolation.ViewModels;

namespace Insolation.Views;

public sealed partial class InsolationView
{
    public InsolationView(InsolationViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}