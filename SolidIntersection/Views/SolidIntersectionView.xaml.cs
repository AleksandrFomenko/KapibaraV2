using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using SolidIntersection.ViewModels;

namespace SolidIntersection.Views;

public sealed partial class SolidIntersectionView: Window
{
    public SolidIntersectionView(SolidIntersectionViewModel viewModel)
    {
        SolidIntersectionViewModel.Close = Close;
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