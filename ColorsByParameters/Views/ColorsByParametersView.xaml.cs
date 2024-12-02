using System.Windows.Media;
using ColorsByParameters.ViewModels;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;


namespace ColorsByParameters.Views;

public sealed partial class ColorsByParametersView
{
    public ColorsByParametersView(ColorsByParametersViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
        InitializeMaterialDesign();
    }
    
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}