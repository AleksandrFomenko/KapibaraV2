using System.Windows.Controls;
using FsmModules.FacadeModule.ViewModel;

namespace FsmModules.FacadeModule.View;

public partial class FacadeModuleView : UserControl
{
    public FacadeModuleView()
    {
        DataContext = new FacadeModuleViewModel();
        InitializeComponent();
    }
}