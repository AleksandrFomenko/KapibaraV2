using KapibaraUI.ProgressBar.viewModel;


namespace KapibaraUI.ProgressBar.view;

public partial class ProgressBarView
{
    public ProgressBarView(ProgressBarViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
    }
}