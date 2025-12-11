using System.Windows;
using KapibaraUI.ProgressBar.viewModel;


namespace KapibaraUI.ProgressBar.view;

public partial class ProgressBarView : Window
{
    public ProgressBarView(ProgressBarViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
    }
}