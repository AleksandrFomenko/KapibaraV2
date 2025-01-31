using ExporterModels.ViewModels;

namespace ExporterModels.Views;

public sealed partial class ExporterModelsView
{
    public ExporterModelsView(ExporterModelsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}