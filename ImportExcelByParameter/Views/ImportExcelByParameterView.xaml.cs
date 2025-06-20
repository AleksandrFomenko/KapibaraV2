using System.Text.RegularExpressions;
using System.Windows.Input;
using ImportExcelByParameter.ViewModels;
using KapibaraUI.Services.Appearance;


namespace ImportExcelByParameter.Views;

public sealed partial class ImportExcelByParameterView
{
    public ImportExcelByParameterView(ImportExcelByParameterViewModel viewModel,
        IThemeWatcherService themeWatcherService)
    {
        DataContext = viewModel;
        themeWatcherService.Watch(this);
        InitializeComponent();
        ImportExcelByParameterViewModel.CloseWindow = this.Close;
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}