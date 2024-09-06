using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using SystemName.ViewModels;

namespace SystemName.Views;

public sealed partial class SystemNameView
{
    public SystemNameView(SystemNameViewModel viewModel)
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