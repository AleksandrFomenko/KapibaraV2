using ImportExcelByParameter.ViewModels;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace ImportExcelByParameter.Views;

public sealed partial class ImportExcelByParameterView
{
    public ImportExcelByParameterView(ImportExcelByParameterViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        InitializeMaterialDesign();
        ImportExcelByParameterViewModel.CloseWindow = this.Close;
    }
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}