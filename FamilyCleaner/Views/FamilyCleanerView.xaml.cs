using System.Windows.Media;
using FamilyCleaner.ViewModels;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FamilyCleaner.Views;

public sealed partial class FamilyCleanerView
{
    public FamilyCleanerView(FamilyCleanerViewModel viewModel)
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