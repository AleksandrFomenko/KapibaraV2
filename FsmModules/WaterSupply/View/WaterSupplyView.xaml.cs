using System.Windows.Controls;
using System.Windows.Media;
using FsmModules.WaterSupply.ViewModel;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FsmModules.WaterSupply.View;

public partial class WaterSupplyView :  UserControl
{
    public WaterSupplyView()
    {
        InitializeMaterialDesign();
        InitializeComponent();
    }
    
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}