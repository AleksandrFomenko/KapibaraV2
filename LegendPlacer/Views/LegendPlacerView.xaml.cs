using System.Text.RegularExpressions;
using System.Windows.Input;
using KapibaraUI.Services.Appearance;
using LegendPlacer.ViewModels;

namespace LegendPlacer.Views;

public sealed partial class LegendPlacerView
{
    public LegendPlacerView(LegendPlacerViewModel viewModel, IThemeWatcherService themeWatcherService)
    {
        themeWatcherService.Watch(this);
        DataContext = viewModel;
        InitializeComponent();
    }
    
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}