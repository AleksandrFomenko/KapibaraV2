using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using SolidIntersector.ViewModels;

namespace SolidIntersector.Views;

public sealed partial class SolidIntersectorView: Window
{
    public SolidIntersectorView(SolidIntersectorViewModel viewModel)
    {
        InitializeMaterialDesign();
        DataContext = viewModel;
        InitializeComponent();
    }
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}