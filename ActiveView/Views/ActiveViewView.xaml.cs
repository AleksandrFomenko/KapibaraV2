using ActiveView.ViewModels;

namespace ActiveView.Views;

public sealed partial class ActiveViewView
{
    public ActiveViewView(ActiveViewViewModel viewModel)
    {
        ActiveViewViewModel.Close = Close;
        DataContext = viewModel;
        InitializeComponent();
    }
}