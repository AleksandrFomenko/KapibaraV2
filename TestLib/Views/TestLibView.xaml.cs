using TestLib.ViewModels;
using WpfResources;

namespace TestLib.Views;

public sealed partial class TestLibView
{
    public TestLibView(TestLibViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
    
}