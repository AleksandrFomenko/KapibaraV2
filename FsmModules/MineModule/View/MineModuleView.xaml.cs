using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FsmModules.MineModule.View;

public partial class MineModuleView : UserControl
{
    public MineModuleView()
    {
        InitializeComponent();
        InitializeMaterialDesign();
    }
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}