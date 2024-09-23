using FsmModules.ViewModels;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FsmModules.Views;

public sealed partial class FsmModulesView
{
    public FsmModulesView(FsmModulesViewModel viewModel)
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