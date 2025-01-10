using System.Text.RegularExpressions;
using System.Windows.Input;
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
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}