using System.Windows.Controls;
using FsmModules.WaterSupply.ViewModel;

namespace FsmModules.WaterSupply.View;

public partial class WaterSupplyView : UserControl
{
    public WaterSupplyView()
    {
        DataContext = new WaterSupplyViewModel();
        InitializeComponent();
    }
}