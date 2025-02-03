using WorkSetLinkFiles.ViewModels;

namespace WorkSetLinkFiles.Views;

public sealed partial class WorkSetLinkFilesView
{
    public WorkSetLinkFilesView(WorkSetLinkFilesViewModel viewModel)
    {
        WorkSetLinkFilesViewModel.Close = Close;
        DataContext = viewModel;
        InitializeComponent();
    }
}